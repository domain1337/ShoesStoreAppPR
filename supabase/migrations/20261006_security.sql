-- Review against the deployed schema before running in Supabase SQL Editor.
-- Roles are stored server-side; user_metadata is never used for authorization.
create table if not exists public.profiles (
  id uuid primary key references auth.users(id) on delete cascade,
  role text not null default 'client' check (role in ('client', 'manager', 'admin'))
);

insert into public.profiles (id, role)
select id, 'client' from auth.users
on conflict (id) do nothing;

create or replace function public.create_user_profile()
returns trigger language plpgsql security definer set search_path = '' as $$
begin
  insert into public.profiles (id, role) values (new.id, 'client')
  on conflict (id) do nothing;
  return new;
end;
$$;

drop trigger if exists on_auth_user_created_profile on auth.users;
create trigger on_auth_user_created_profile after insert on auth.users
for each row execute function public.create_user_profile();

create or replace function public.current_store_role()
returns text language sql stable security definer set search_path = '' as $$
  select coalesce((select role from public.profiles where id = (select auth.uid())), 'guest');
$$;

revoke all on function public.current_store_role() from public;
grant execute on function public.current_store_role() to anon, authenticated;
-- Remove earlier permissive policies: PostgreSQL combines policies with OR.
do $$
declare policy_row record;
begin
  for policy_row in
    select schemaname, tablename, policyname from pg_policies
    where schemaname = 'public' and tablename in ('profiles', 'products', 'pickup_points', 'orders')
  loop
    execute format('drop policy %I on %I.%I', policy_row.policyname, policy_row.schemaname, policy_row.tablename);
  end loop;
end;
$$;
revoke all on public.profiles from anon, authenticated;
grant select on public.profiles to authenticated;
alter table public.profiles enable row level security;
create policy profiles_read_self on public.profiles for select to authenticated
using (id = (select auth.uid()));

alter table public.products enable row level security;
create policy products_read on public.products for select to anon, authenticated using (true);
create policy products_admin_insert on public.products for insert to authenticated
with check ((select public.current_store_role()) = 'admin');
create policy products_admin_update on public.products for update to authenticated
using ((select public.current_store_role()) = 'admin')
with check ((select public.current_store_role()) = 'admin');
create policy products_admin_delete on public.products for delete to authenticated
using ((select public.current_store_role()) = 'admin');

alter table public.pickup_points enable row level security;
create policy pickup_points_read on public.pickup_points for select to anon, authenticated using (true);

alter table public.orders enable row level security;
create policy orders_read on public.orders for select to authenticated
using ((select public.current_store_role()) in ('admin', 'manager')
  or lower(customer_email) = lower((select auth.jwt() ->> 'email')));
create policy orders_insert_own on public.orders for insert to authenticated
with check (lower(customer_email) = lower((select auth.jwt() ->> 'email'))
  and total_price > 0 and status = 'Новый');

-- Storage metadata is protected too; the images bucket remains publicly readable.
drop policy if exists store_images_admin_insert on storage.objects;
drop policy if exists store_images_admin_update on storage.objects;
drop policy if exists store_images_admin_delete on storage.objects;
drop policy if exists store_images_admin_insert_guard on storage.objects;
drop policy if exists store_images_admin_update_guard on storage.objects;
drop policy if exists store_images_admin_delete_guard on storage.objects;
create policy store_images_admin_insert on storage.objects for insert to authenticated
with check (bucket_id = 'images' and (select public.current_store_role()) = 'admin');
create policy store_images_admin_update on storage.objects for update to authenticated
using (bucket_id = 'images' and (select public.current_store_role()) = 'admin')
with check (bucket_id = 'images' and (select public.current_store_role()) = 'admin');
create policy store_images_admin_delete on storage.objects for delete to authenticated
using (bucket_id = 'images' and (select public.current_store_role()) = 'admin');
-- Restrictive guards also apply if an older permissive storage policy remains.
create policy store_images_admin_insert_guard on storage.objects as restrictive for insert to public
with check (bucket_id <> 'images' or (select public.current_store_role()) = 'admin');
create policy store_images_admin_update_guard on storage.objects as restrictive for update to public
using (bucket_id <> 'images' or (select public.current_store_role()) = 'admin')
with check (bucket_id <> 'images' or (select public.current_store_role()) = 'admin');
create policy store_images_admin_delete_guard on storage.objects as restrictive for delete to public
using (bucket_id <> 'images' or (select public.current_store_role()) = 'admin');
