using System.Collections.Generic;

namespace NTECoffeeShop.CoffeeShop
{
    internal class DataModel
    {
        public Dictionary<string, List<ETemplateName>> DefaultStageProducts = new Dictionary<string, List<ETemplateName>>();

        public static readonly List<ETemplateName> Stage_All = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.CookieSugarCoffee,
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.AppleSauceCake,
            ETemplateName.CandyStickCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.ChocolateChipsCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.FriedChickenToast,
            ETemplateName.TomatoHamToast,

            ETemplateName.CheeseHamCroissant,
            ETemplateName.CheesePomegranateCroissant,
            ETemplateName.TomatoEggCroissant,
        };

        #region 章节 1

        private readonly List<ETemplateName> Stage_1_0 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
        };

        private readonly List<ETemplateName> Stage_1_1 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_2 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_3 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_4 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_5 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_6 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_7 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_8 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_9 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_1_10 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.AppleSauceCake,
            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        #endregion

        #region 章节 2

        private readonly List<ETemplateName> Stage_2_1 = new List<ETemplateName>()
        {
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_2_2 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.AppleSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_2_3 = new List<ETemplateName>()
        {
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.AppleSauceCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.TomatoHamToast,
            ETemplateName.TomatoEggCroissant,
        };

        private readonly List<ETemplateName> Stage_2_4 = new List<ETemplateName>()
        {
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_2_5 = new List<ETemplateName>()
        {
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_2_6 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.AppleSauceCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.TomatoHamToast,
            ETemplateName.BeafOnionToast,

            ETemplateName.TomatoEggCroissant,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_2_7 = new List<ETemplateName>()
        {
            ETemplateName.AppleSauceCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,
        };

        private readonly List<ETemplateName> Stage_2_8 = new List<ETemplateName>()
        {
            ETemplateName.ChocolateJamCoffee,
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.AppleSauceCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,
        };

        private readonly List<ETemplateName> Stage_2_9 = new List<ETemplateName>()
        {
            ETemplateName.CheeseMilkCoffee,
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.AppleSauceCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.TomatoHamToast,
            ETemplateName.BeafOnionToast,

            ETemplateName.TomatoEggCroissant,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_2_10 = new List<ETemplateName>()
        {
            ETemplateName.ChocolateCreamCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.CheesePomegranateCroissant,
        };

        #endregion

        #region 章节 3

        private readonly List<ETemplateName> Stage_3_1 = new List<ETemplateName>()
        {
            ETemplateName.CookieSugarCoffee,
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CandyStickCake,
            ETemplateName.ChocolateChipsCake,

            ETemplateName.FriedChickenToast,
            ETemplateName.CheeseHamCroissant,
        };

        private readonly List<ETemplateName> Stage_3_2 = new List<ETemplateName>()
        {
            ETemplateName.CookieSugarCoffee,
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CandyStickCake,
            ETemplateName.ChocolateChipsCake,

            ETemplateName.FriedChickenToast,
            ETemplateName.CheeseHamCroissant,
        };

        private readonly List<ETemplateName> Stage_3_3 = new List<ETemplateName>()
        {
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CandyStickCake,
            ETemplateName.ChocolateChipsCake,
        };

        private readonly List<ETemplateName> Stage_3_4 = new List<ETemplateName>()
        {
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CandyStickCake,
            ETemplateName.ChocolateChipsCake,

            ETemplateName.FriedChickenToast,
        };

        private readonly List<ETemplateName> Stage_3_5 = new List<ETemplateName>()
        {
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CandyStickCake,
            ETemplateName.ChocolateChipsCake,

            ETemplateName.FriedChickenToast,
            ETemplateName.CheeseHamCroissant,
        };

        private readonly List<ETemplateName> Stage_3_6 = new List<ETemplateName>()
        {
            ETemplateName.CookieSugarCoffee,
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.CandyStickCake,
            ETemplateName.ChocolateChipsCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.FriedChickenToast,

            ETemplateName.CheeseHamCroissant,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_3_7 = new List<ETemplateName>()
        {
            ETemplateName.CheeseSauceCake,
            ETemplateName.ChocolateChipsCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.FriedChickenToast,
            ETemplateName.TomatoHamToast,
        };

        private readonly List<ETemplateName> Stage_3_8 = new List<ETemplateName>()
        {
            ETemplateName.DriedTangerineCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.ChocolateChipsCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.FriedChickenToast,
            ETemplateName.TomatoHamToast,
        };

        private readonly List<ETemplateName> Stage_3_9 = new List<ETemplateName>()
        {
            ETemplateName.CookieSugarCoffee,
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberryGuacamoleCoffee,

            ETemplateName.CandyStickCake,
            ETemplateName.CheeseSauceCake,
            ETemplateName.ChocolateChipsCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.FriedChickenToast,

            ETemplateName.CheeseHamCroissant,
            ETemplateName.CheesePomegranateCroissant,
        };

        private readonly List<ETemplateName> Stage_3_10 = new List<ETemplateName>()
        {
            ETemplateName.CookieSugarCoffee,
            ETemplateName.DriedTangerineCoffee,
            ETemplateName.StrawberrySugarCoffee,

            ETemplateName.CheeseSauceCake,
            ETemplateName.StrawberryJamCake,

            ETemplateName.BeafOnionToast,
            ETemplateName.FriedChickenToast,

            ETemplateName.CheeseHamCroissant,
            ETemplateName.CheesePomegranateCroissant,
        };

        #endregion

        public DataModel()
        {
            SetDefaultStageData();
        }

        public void SetDefaultStageData()
        {
            DefaultStageProducts.Add("1-0", Stage_1_0);
            DefaultStageProducts.Add("1-1", Stage_1_1);
            DefaultStageProducts.Add("1-2", Stage_1_2);
            DefaultStageProducts.Add("1-3", Stage_1_3);
            DefaultStageProducts.Add("1-4", Stage_1_4);
            DefaultStageProducts.Add("1-5", Stage_1_5);
            DefaultStageProducts.Add("1-6", Stage_1_6);
            DefaultStageProducts.Add("1-7", Stage_1_7);
            DefaultStageProducts.Add("1-8", Stage_1_8);
            DefaultStageProducts.Add("1-9", Stage_1_9);
            DefaultStageProducts.Add("1-10", Stage_1_10);

            DefaultStageProducts.Add("2-1", Stage_2_1);
            DefaultStageProducts.Add("2-2", Stage_2_2);
            DefaultStageProducts.Add("2-3", Stage_2_3);
            DefaultStageProducts.Add("2-4", Stage_2_4);
            DefaultStageProducts.Add("2-5", Stage_2_5);
            DefaultStageProducts.Add("2-6", Stage_2_6);
            DefaultStageProducts.Add("2-7", Stage_2_7);
            DefaultStageProducts.Add("2-8", Stage_2_8);
            DefaultStageProducts.Add("2-9", Stage_2_9);
            DefaultStageProducts.Add("2-10", Stage_2_10);

            DefaultStageProducts.Add("3-1", Stage_3_1);
            DefaultStageProducts.Add("3-2", Stage_3_2);
            DefaultStageProducts.Add("3-3", Stage_3_3);
            DefaultStageProducts.Add("3-4", Stage_3_4);
            DefaultStageProducts.Add("3-5", Stage_3_5);
            DefaultStageProducts.Add("3-6", Stage_3_6);
            DefaultStageProducts.Add("3-7", Stage_3_7);
            DefaultStageProducts.Add("3-8", Stage_3_8);
            DefaultStageProducts.Add("3-9", Stage_3_9);
            DefaultStageProducts.Add("3-10", Stage_3_10);
        }
    }
}
