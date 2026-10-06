using System.Net.Mail;
using ShoesStoreApp.Models;

namespace ShoesStoreApp.Services;

public static class StoreValidation
{
    public static string? Account(string? email, string? password, bool registration)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return "Введите адрес электронной почты и пароль.";
        if (!MailAddress.TryCreate(email.Trim(), out var parsed) || parsed.Address != email.Trim())
            return "Введите корректный адрес электронной почты.";
        if (registration && password.Length < 8)
            return "Пароль должен содержать не менее 8 символов.";
        return null;
    }

    public static string? Product(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Title) || string.IsNullOrWhiteSpace(product.Article))
            return "Укажите название и артикул товара.";
        if (product.Price <= 0 || product.Discount is < 0 or > 100 || product.QuantityInStock < 0)
            return "Цена должна быть больше нуля, скидка — от 0 до 100%, остаток — неотрицательным.";
        return null;
    }

    public static string? Checkout(bool authenticated, int itemCount, PickupPoint? pickupPoint)
    {
        if (!authenticated) return "Оформление заказа доступно только авторизованным пользователям.";
        if (itemCount == 0) return "Ваша корзина пуста.";
        if (pickupPoint is null) return "Выберите пункт выдачи.";
        return null;
    }
}
