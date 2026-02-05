using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Core.Constants
{
    public static class RegularExpressionsSettings
    {
        public static string ArabicName = @"^[\u0621-\u064A ]+$";
        public static string EnglishName = @"^[a-zA-Z ]+$";
        public static string OnlyNumbers = @"^\d+$";
        public static string EmailAddress = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$|^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}";
        public static string ComplexPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).\S{7,}$";
    }
}
