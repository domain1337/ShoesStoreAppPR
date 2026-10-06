using ShoesStoreApp.Models;
using ShoesStoreApp.Services;

var checks = new (string Name, Func<bool> Test)[]
{
    ("Некорректная почта", () => StoreValidation.Account("wrong", "12345678", true) != null),
    ("Короткий пароль", () => StoreValidation.Account("a@b.ru", "123", true) != null),
    ("Корректная регистрация", () => StoreValidation.Account("a@b.ru", "12345678", true) == null),
    ("Пустое название", () => StoreValidation.Product(new Product { Article = "A", Price = 10 }) != null),
    ("Отрицательная цена", () => StoreValidation.Product(new Product { Article = "A", Title = "Boot", Price = -1 }) != null),
    ("Скидка больше 100", () => StoreValidation.Product(new Product { Article = "A", Title = "Boot", Price = 10, Discount = 101 }) != null),
    ("Отрицательный остаток", () => StoreValidation.Product(new Product { Article = "A", Title = "Boot", Price = 10, QuantityInStock = -1 }) != null),
    ("Корректный товар", () => StoreValidation.Product(new Product { Article = "A", Title = "Boot", Price = 10, Discount = 20 }) == null),
    ("Итоговая цена", () => new Product { Price = 100m, Discount = 25m }.FinalPrice == 75m),
    ("Гость не оформляет заказ", () => StoreValidation.Checkout(false, 1, new PickupPoint()) != null),
    ("Пустая корзина", () => StoreValidation.Checkout(true, 0, new PickupPoint()) != null),
    ("Нужен пункт выдачи", () => StoreValidation.Checkout(true, 1, null) != null),
    ("Корректное оформление", () => StoreValidation.Checkout(true, 1, new PickupPoint()) == null),
    ("Роль не повышается из неизвестного значения", () => CheckRole("unexpected", false)),
    ("Роль администратора", () => CheckRole("admin", true))
};

var failed = 0;
foreach (var (name, test) in checks)
{
    var ok = test();
    Console.WriteLine($"{(ok ? "OK" : "FAIL")}: {name}");
    if (!ok) failed++;
}
return failed == 0 ? 0 : 1;

static bool CheckRole(string role, bool expected)
{
    UserService.SetAuthenticated("a@b.ru", role);
    var result = UserService.IsAdmin == expected;
    UserService.Reset();
    return result;
}
