using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using MoqqerNamespace.MoqqerQueryable;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.Helpers
{
    [TestFixture]
    internal class QueryableTestTests
    {
        [SetUp]
        public void A_TestInitialise()
        {
            _moq = new Moqqer();
        }

        private Moqqer _moq;

        private class Level0
        {
            public Level1 L1 { get; set; }
        }

        private class Level1
        {
            public Level2 L2 { get; set; }
        }

        private class Level2
        {
            public Level3 L3 { get; set; }
        }

        private class Level3
        {
            public Level4 L4 { get; set; }
        }

        private class Level4
        {
            public Level5 L5 { get; set; }
        }

        private class Level5
        {
            public string Name { get; set; }
            public decimal Decimal100 { get; set; }
            public int Integer100 { get; set; }
            public int? NullableInteger { get; set; }
            public decimal? NullableDecimal { get; set; }
            public bool Boolean { get; set; }
        }

        private Level0 SingleLevel => new Level0();

        private Level0 AllLevels => new
            Level0
            {
                L1 = new Level1
                {
                    L2 = new Level2
                    {
                        L3 = new Level3
                        {
                            L4 = new Level4
                            {
                                L5 = new Level5
                                {
                                    Name = "Name",
                                    Decimal100 = 100,
                                    Integer100 = 100
                                }
                            }
                        }
                    }
                }
            };

        private Level0 GetItem(string key)
        {
            switch (key)
            {
                case nameof(AllLevels):
                    return AllLevels;
                case nameof(SingleLevel):
                    return SingleLevel;
                default:
                    throw new Exception();
            }
        }

        private List<Level0> GetItems(string key, Action<Level0> itemAction = null)
        {
            var level0 = GetItem(key);

            try
            {
                itemAction?.Invoke(level0);
            }
            catch
            {
                // ignored
            }


            return new List<Level0> {level0};
        }

        private EnumerableMoqqerQuery<Level0> GetQueryable(string key, Action<Level0> action = null)
        {
            var list = GetItems(key, action);

            var queryable = new EnumerableMoqqerQuery<Level0>(list);
            return queryable;
        }

        [Test]
        public void EnumerableMoqqerQuery_Select_AllLevels_IntoMultiPropertyAnnonymousType()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            var q = queryable.Select(x => new
            {
                FirstProperty = x.L1.L2.L3.L4.L5.Name,
                SecondProperty = x.L1.L2,
                ConstantProperty = 25,
                StringProeprty = "Test",
                MethodCallProperty = Math.Abs(-69)
            });

            var res = q.First();

            res.FirstProperty.Should().Be("Name");
            res.SecondProperty.Should().NotBe(null);
            res.ConstantProperty.Should().Be(25);
            res.StringProeprty.Should().Be("Test");
            res.MethodCallProperty.Should().Be(69);
        }

        [Test]
        [TestCase(nameof(AllLevels), 200)]
        [TestCase(nameof(SingleLevel), null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Add_Integer_Plus_Decimal
            (string key, decimal? expected)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Select(x => (int?) x.L1.L2.L3.L4.L5.Integer100 + x.L1.L2.L3.L4.L5.Decimal100);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 5, 5, 10)]
        [TestCase(nameof(AllLevels), null, null, null)]
        [TestCase(nameof(AllLevels), null, 5, null)]
        [TestCase(nameof(SingleLevel), null, null, null)]
        [TestCase(nameof(SingleLevel), null, 5, null)]
        [TestCase(nameof(SingleLevel), 5, 5, null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Add_NullableInteger_Plus_NullableDecimal
            (string key, int? integer, decimal? dec, decimal? expected)
        {
            var queryable = integer != null || dec != null
                ? GetQueryable(key, x =>
                {
                    x.L1.L2.L3.L4.L5.NullableInteger = integer;
                    x.L1.L2.L3.L4.L5.NullableDecimal = dec;
                })
                : GetQueryable(key);

            var q = queryable.Select(x => x.L1.L2.L3.L4.L5.NullableInteger + x.L1.L2.L3.L4.L5.NullableDecimal);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 0)]
        [TestCase(nameof(SingleLevel), null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Subtract_Integer_Plus_Decimal
            (string key, decimal? expected)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Select(x => (int?) x.L1.L2.L3.L4.L5.Integer100 - x.L1.L2.L3.L4.L5.Decimal100);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 10, 7, 3)]
        [TestCase(nameof(AllLevels), null, null, null)]
        [TestCase(nameof(AllLevels), null, 5, null)]
        [TestCase(nameof(SingleLevel), null, null, null)]
        [TestCase(nameof(SingleLevel), null, 5, null)]
        [TestCase(nameof(SingleLevel), 5, 5, null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Subtract_NullableInteger_Plus_NullableDecimal
            (string key, int? integer, decimal? dec, decimal? expected)
        {
            var queryable = integer != null || dec != null
                ? GetQueryable(key, x =>
                {
                    x.L1.L2.L3.L4.L5.NullableInteger = integer;
                    x.L1.L2.L3.L4.L5.NullableDecimal = dec;
                })
                : GetQueryable(key);

            var q = queryable.Select(x => x.L1.L2.L3.L4.L5.NullableInteger - x.L1.L2.L3.L4.L5.NullableDecimal);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 100)]
        [TestCase(nameof(SingleLevel), null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Convert_Interger_ToNullableInteger
            (string key, int? expected)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Select(x => (int?) x.L1.L2.L3.L4.L5.Integer100);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 10000)]
        [TestCase(nameof(SingleLevel), null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Multiply_Integer_Plus_Decimal
            (string key, decimal? expected)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Select(x => (int?) x.L1.L2.L3.L4.L5.Integer100 * x.L1.L2.L3.L4.L5.Decimal100);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 5, 5, 25)]
        [TestCase(nameof(AllLevels), null, null, null)]
        [TestCase(nameof(AllLevels), null, 5, null)]
        [TestCase(nameof(SingleLevel), null, null, null)]
        [TestCase(nameof(SingleLevel), null, 5, null)]
        [TestCase(nameof(SingleLevel), 5, 5, null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Multiply_NullableInteger_Plus_NullableDecimal
            (string key, int? integer, decimal? dec, decimal? expected)
        {
            var queryable = integer != null || dec != null
                ? GetQueryable(key, x =>
                {
                    x.L1.L2.L3.L4.L5.NullableInteger = integer;
                    x.L1.L2.L3.L4.L5.NullableDecimal = dec;
                })
                : GetQueryable(key);

            var q = queryable.Select(x => x.L1.L2.L3.L4.L5.NullableInteger * x.L1.L2.L3.L4.L5.NullableDecimal);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 1)]
        [TestCase(nameof(SingleLevel), null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Divide_Integer_Plus_Decimal
            (string key, decimal? expected)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Select(x => (int?) x.L1.L2.L3.L4.L5.Integer100 / x.L1.L2.L3.L4.L5.Decimal100);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 5, 5, 1)]
        [TestCase(nameof(AllLevels), null, null, null)]
        [TestCase(nameof(AllLevels), null, 5, null)]
        [TestCase(nameof(SingleLevel), null, null, null)]
        [TestCase(nameof(SingleLevel), null, 5, null)]
        [TestCase(nameof(SingleLevel), 5, 5, null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Divide_NullableInteger_Plus_NullableDecimal
            (string key, int? integer, decimal? dec, decimal? expected)
        {
            var queryable = integer != null || dec != null
                ? GetQueryable(key, x =>
                {
                    x.L1.L2.L3.L4.L5.NullableInteger = integer;
                    x.L1.L2.L3.L4.L5.NullableDecimal = dec;
                })
                : GetQueryable(key);

            var q = queryable.Select(x => x.L1.L2.L3.L4.L5.NullableInteger / x.L1.L2.L3.L4.L5.NullableDecimal);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 0)]
        [TestCase(nameof(SingleLevel), null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Module_Integer_And_Decimal
            (string key, decimal? expected)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Select(x => (int?) x.L1.L2.L3.L4.L5.Integer100 % x.L1.L2.L3.L4.L5.Decimal100);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        [TestCase(nameof(AllLevels), 5, 3, 2)]
        [TestCase(nameof(AllLevels), null, null, null)]
        [TestCase(nameof(AllLevels), null, 5, null)]
        [TestCase(nameof(SingleLevel), null, null, null)]
        [TestCase(nameof(SingleLevel), null, 5, null)]
        [TestCase(nameof(SingleLevel), 5, 5, null)]
        public void EnumerableMoqqerQuery_Select_ExpressionType_Modulo_NullableInteger_And_NullableDecimal
            (string key, int? integer, decimal? dec, decimal? expected)
        {
            var queryable = integer != null || dec != null
                ? GetQueryable(key, x =>
                {
                    x.L1.L2.L3.L4.L5.NullableInteger = integer;
                    x.L1.L2.L3.L4.L5.NullableDecimal = dec;
                })
                : GetQueryable(key);

            var q = queryable.Select(x => x.L1.L2.L3.L4.L5.NullableInteger % x.L1.L2.L3.L4.L5.NullableDecimal);

            var res = q.FirstOrDefault();

            res.Should().Be(expected);
        }

        [Test]
        public void EnumerableMoqqerQuery_Select_SingleLevel_IntoMultiPropertyAnnonymousType()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            var q = queryable.Select(x => new
            {
                FirstProperty = x.L1.L2.L3.L4.L5.Name,
                SecondProperty = x.L1.L2,
                ConstantProperty = 25,
                StringProeprty = "Test",
                MethodCallProperty = Math.Abs(-69)
            });

            var res = q.First();

            res.FirstProperty.Should().Be(null);
            res.SecondProperty.Should().Be(null);
            res.ConstantProperty.Should().Be(25);
            res.StringProeprty.Should().Be("Test");
            res.MethodCallProperty.Should().Be(69);
        }

        [Test]
        public void EnumerableMoqqerQuery_Select_SingleLevel_IntoSinglePropertyAnnonymousType()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            var q = queryable.Select(x => new {SingleProperty = x.L1.L2.L3.L4.L5.Name});

            var res = q.First();

            res.SingleProperty.Should().Be(null);
        }

        [Test]
        public void EnumerableMoqqerQuery_Select_SingleNestedProperty()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            var q = queryable.Select(x => x.L1.L2.L3.L4.L5.Name);

            var res = q.FirstOrDefault();

            res.Should().Be(null);
        }

        [Test]
        public void
            EnumerableMoqqerQuery_Where_ExpressionType_AndAlso_AllLevels_NameComparisonToNotNull_And_DecimalLessThan100()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name != null && x.L1.L2.L3.L4.L5.Decimal100 < 100);

            var res = q.FirstOrDefault();

            res.Should().BeNull("Decimal conditions not met");
        }

        [Test]
        public void
            EnumerableMoqqerQuery_Where_ExpressionType_AndAlso_AllLevels_NameComparisonToNotNull_And_DecimalMoreThan100()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name != null && x.L1.L2.L3.L4.L5.Decimal100 > 99);

            var res = q.FirstOrDefault();

            res.Should().NotBeNull("as all levels meets conditions");
        }


        [Test]
        public void
            EnumerableMoqqerQuery_Where_ExpressionType_AndAlso_AllLevels_NameComparisonToNull_And_DecimalLessThan100()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == null && x.L1.L2.L3.L4.L5.Decimal100 < 100);

            var res = q.FirstOrDefault();

            res.Should().BeNull("conditions not met");
        }

        [Test]
        public void
            EnumerableMoqqerQuery_Where_ExpressionType_AndAlso_AllLevels_NameComparisonToNull_And_DecimalMoreThan100()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == null && x.L1.L2.L3.L4.L5.Decimal100 < 100);

            var res = q.FirstOrDefault();

            res.Should().BeNull("Decimal condition not met");
        }

        [Test]
        public void
            EnumerableMoqqerQuery_Where_ExpressionType_AndAlso_SingleLevel_NameComparisonToNull_And_DecimalLessThan100()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == null && x.L1.L2.L3.L4.L5.Decimal100 < 100);

            var res = q.FirstOrDefault();

            res.Should().BeNull("as deepest level doesn't even exist");
        }

        [Test]
        [TestCase(nameof(AllLevels), null, 0, 0, false)]
        [TestCase(nameof(AllLevels), null, 200, 0, false)]
        [TestCase(nameof(AllLevels), null, 0, 200, true)]
        [TestCase(nameof(AllLevels), null, 200, 200, true)]
        [TestCase(nameof(AllLevels), "", 0, 0, false)]
        [TestCase(nameof(AllLevels), "", 200, 0, false)]
        [TestCase(nameof(AllLevels), "", 0, 200, true)]
        [TestCase(nameof(AllLevels), "", 200, 200, true)]
        [TestCase(nameof(AllLevels), "Name", 0, 0, false)]
        [TestCase(nameof(AllLevels), "Name", 200, 0, true)]
        [TestCase(nameof(AllLevels), "Name", 0, 200, false)]
        [TestCase(nameof(AllLevels), "Name", 200, 200, true)]
        [TestCase(nameof(SingleLevel), null, 0, 0, false)]
        [TestCase(nameof(SingleLevel), null, 200, 0, false)]
        [TestCase(nameof(SingleLevel), null, 0, 200, false)]
        [TestCase(nameof(SingleLevel), null, 200, 200, false)]
        [TestCase(nameof(SingleLevel), "", 0, 0, false)]
        [TestCase(nameof(SingleLevel), "", 200, 0, false)]
        [TestCase(nameof(SingleLevel), "", 0, 200, false)]
        [TestCase(nameof(SingleLevel), "", 200, 200, false)]
        [TestCase(nameof(SingleLevel), "Name", 0, 0, false)]
        [TestCase(nameof(SingleLevel), "Name", 200, 0, false)]
        [TestCase(nameof(SingleLevel), "Name", 0, 200, false)]
        [TestCase(nameof(SingleLevel), "Name", 200, 200, false)]
        public void
            EnumerableMoqqerQuery_Where_ExpressionType_Conditional_NameEqualsComparison_TrueChecksIntegerLessThanComparison_FalseChecksDecimalLessThanComparison
            (string key, string name, int integer, int dec, bool shouldReturnItem)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == name
                ? x.L1.L2.L3.L4.L5.Integer100 < integer
                : x.L1.L2.L3.L4.L5.Decimal100 < dec);

            var res = q.FirstOrDefault();

            if (shouldReturnItem)
                res.Should().NotBeNull();
            else
                res.Should().BeNull();
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_Equal_AllLevels_ComaprisonReturnsFalse()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == "Test");

            var res = q.FirstOrDefault();

            res.Should().BeNull();
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_Equal_AllLevels_ComaprisonReturnsTrue()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == "Name");

            var res = q.FirstOrDefault();

            res.Should().NotBeNull();
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_Equal_SingleLevel()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == "Name");

            var res = q.FirstOrDefault();

            res.Should().BeNull("as deepest level doesn't even exist");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_Equal_SingleLevel_ComparisonToNull()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == null);

            var res = q.FirstOrDefault();

            res.Should().BeNull("as deepest level doesn't even exist");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_GreaterThan_AllLevels()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Decimal100 > 25);

            var res = q.FirstOrDefault();

            res.Should().NotBeNull("All Levels has decimal of 100");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_GreaterThan_SingleLevel()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Decimal100 > 25);

            var res = q.FirstOrDefault();

            res.Should().BeNull("single level does not have non null references until decimal");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_GreaterThanOrEqual_SingleLevel()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Decimal100 >= 25);

            var res = q.FirstOrDefault();

            res.Should().BeNull("single level does not have L5.Decimal");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_LessThan_SingleLevel()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Decimal100 < 25);

            var res = q.FirstOrDefault();

            res.Should().BeNull("Single Level");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_LessThanOrEqual_SingleLevel()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Decimal100 <= 25);

            var res = q.FirstOrDefault();

            res.Should().BeNull("single level.");
        }

        [Test]
        [TestCase(nameof(AllLevels), false, true)]
        [TestCase(nameof(AllLevels), true, false)]
        [TestCase(nameof(SingleLevel), null, false)]
        public void EnumerableMoqqerQuery_Where_ExpressionType_Not
            (string key, bool? boolean, bool shouldReturnItem)
        {
            var queryable = boolean.HasValue
                ? GetQueryable(key, x => x.L1.L2.L3.L4.L5.Boolean = boolean.Value)
                : GetQueryable(key);

            var q = queryable.Where(x => !x.L1.L2.L3.L4.L5.Boolean);

            var res = q.FirstOrDefault();

            if (shouldReturnItem)
                res.Should().NotBeNull();
            else
                res.Should().BeNull();
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_NotEqual_AllLevels()
        {
            var list = new List<Level0> {AllLevels};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name != null);

            var res = q.FirstOrDefault();

            res.Should().NotBeNull("AllLevels has Name");
        }

        [Test]
        public void EnumerableMoqqerQuery_Where_ExpressionType_NotEqual_SingleLevel()
        {
            var list = new List<Level0> {new Level0()};

            var queryable = new EnumerableMoqqerQuery<Level0>(list);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name != null);

            var res = q.FirstOrDefault();

            res.Should().BeNull("Single Level");
        }

        [Test]
        [TestCase(nameof(AllLevels), null, 200, true)]
        [TestCase(nameof(AllLevels), "Name", 200, true)]
        [TestCase(nameof(AllLevels), "Name", 0, true)]
        [TestCase(nameof(AllLevels), "Othername", 0, false)]
        [TestCase(nameof(SingleLevel), null, 200, false)]
        [TestCase(nameof(SingleLevel), "Name", 200, false)]
        [TestCase(nameof(SingleLevel), "Name", 0, false)]
        [TestCase(nameof(SingleLevel), "Othername", 0, false)]
        public void EnumerableMoqqerQuery_Where_ExpressionType_OrElse_NameEqualsComparison_And_IntegerLessThanComparison
            (string key, string name, int integer, bool shouldReturnItem)
        {
            var queryable = GetQueryable(key);

            var q = queryable.Where(x => x.L1.L2.L3.L4.L5.Name == name || x.L1.L2.L3.L4.L5.Integer100 < integer);

            var res = q.FirstOrDefault();

            if (shouldReturnItem)
                res.Should().NotBeNull();
            else
                res.Should().BeNull();
        }


        [Test]
        public void Method_State_Result()
        {
            Expression<Func<Level0, string>> expr1 = x => x.L1.L2.L3.L4.L5.Name;

            var expr = MoqqerExpressionRewriter.RebuildExpressStronglyTypes(expr1);

            var compiled = expr.Compile();

            // Valid Object
            compiled(AllLevels)
                .Should()
                .Be("Name");

            // Missing Object
            compiled(new Level0())
                .Should()
                .Be(null);
        }
    }
}