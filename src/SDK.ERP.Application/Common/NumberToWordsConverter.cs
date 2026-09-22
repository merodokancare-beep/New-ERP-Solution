namespace SDK.ERP.Application.Common;

public static class NumberToWordsConverter
{
    private static readonly string[] UnitsMap = 
    {
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    };

    private static readonly string[] TensMap = 
    {
        "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

    public static string ConvertToIndianWords(decimal amount)
    {
        if (amount == 0)
            return "INR Zero Only";

        long rupees = (long)Math.Floor(amount);
        int paise = (int)Math.Round((amount - rupees) * 100);

        string words = "INR " + NumberToWords(rupees).Trim();

        if (paise > 0)
        {
            words += " and " + NumberToWords(paise).Trim() + " Paise";
        }

        return words + " Only";
    }

    private static string NumberToWords(long number)
    {
        if (number == 0)
            return "";

        if (number < 20)
            return UnitsMap[number];

        if (number < 100)
            return TensMap[number / 10] + ((number % 10 > 0) ? " " + UnitsMap[number % 10] : "");

        if (number < 1000)
            return UnitsMap[number / 100] + " Hundred" + ((number % 100 > 0) ? " and " + NumberToWords(number % 100) : "");

        if (number < 100000)
            return NumberToWords(number / 1000) + " Thousand" + ((number % 1000 > 0) ? " " + NumberToWords(number % 1000) : "");

        if (number < 10000000)
            return NumberToWords(number / 100000) + " Lakh" + ((number % 100000 > 0) ? " " + NumberToWords(number % 100000) : "");

        return NumberToWords(number / 10000000) + " Crore" + ((number % 10000000 > 0) ? " " + NumberToWords(number % 10000000) : "");
    }
}
