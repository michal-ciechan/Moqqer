# Moqqer

<!-- TOC depthFrom:1 depthTo:6 withLinks:1 updateOnSave:1 orderedList:0 -->

- [Moqqer](#moqqer)
- [Intro](#intro)
- [Usage](#usage)
- [Example](#example)
- [Features](#features)
	- [Constructor Injection](#constructor-injection)
	- [Default Object Injection](#default-object-injection)
	- [Recursive Mocking](#recursive-mocking)
	- [Lazy Mocking](#lazy-mocking)
- [Non Mockable Defaults](#non-mockable-defaults)
- [Installing](#installing)
- [Roadmap](#roadmap)
	- [Quicker Verification:](#quicker-verification)
	- [Easier Setup](#easier-setup)

<!-- /TOC -->

# Intro

Moqqer is an AutoMocker for Moq.

It automatically injects mocks into the constructor of a class and lets you retrieve the injected mocks later for verification.

This helps to ease the pain of creating mocks each time, as well as time saving when you change the constructors or refactor code.

Moqqer also automatically stubs methods of an interface to return Mocks of the return type or some defaults (where applicable). See the section on default Mock value below for more info.

# Usage

1 -  First create an instance of Moqqer which will act as a container for Mocks and Objects.


    _moq = new Moqqer();

2 -   Get Moqqer to create an instance of some class you want to test. It will auto inject mocks in the its constructor.


    _subject = _moq.Create<SomeClass>();

 Call the mothod you want to test on SomeClass


    _subject.CallA();

3 - Verify a mock that was auto injected was called.


    _moq.Of<IDepencyA>()
        .Verify(x => x.Call(), Times.Once);

4 - Test and Refactor to your hearts content without worrying about changing the constructor calls!

# Example

    using Moqqer.Namespace

    [TestFixture]
    public class IntroExampleTests
    {
        private Moqqer _moq;
        private SomeClass _subject;****

        // Setup run per test.
        [SetUp]
        public void A_TestInit()
        {
            // 1. Create instance of Moqqer
            _moq = new Moqqer();

            // Create an instance of SomeClass
            _subject = _moq.Create<SomeClass>();
        }

        [Test]
        public void DoWork_CallsRepositoryGetData()
        {
            _subject.CallA(); // Calls private field IDependencyA.Call()

            _moq.Of<IDepencyA>()
                .Verify(x => x.Call(), Times.Once);
        }
    }

# Features

## Constructor Injection

Use the `Moqqer.Create<T>()` method  in order to get Moqqer to create an instance of `<T>` and inject Mocks in the constructor.

    var subject = _moq.Create<SomeClass>();

    var depencyA = _moq.Of<IDepencyA>().Object;
    var depencyB = _moq.Of<IDepencyB>().Object;
    var expected = _moq.Of<IMockSetup>().Object;

    subject.A.Should().BeSameAs(depencyA);
    subject.B.Should().BeSameAs(depencyB);
    subject.Mock.Should().BeSameAs(expected);

## Default Object Injection

By default Moqqer will inject Mocks into types which are _Mockable_. For types which aren't Mockable, Moqqer will resolve those to the type or object as per the [Non Mockable Defaults](#non-mockable-defaults) below.

    public class StringConstructorClass
    {
        public string Text { get; set; }

        public StringConstructorClass(string text)
        {
            Text = text;
        }
    }

    [Test]
    public void DefaultObjectInjection()
    {
        var subject = _moq.Create<StringConstructorClass>();

        subject.Text.Should().Be(string.Empty);
    }

## Recursive Mocking

As Moqqer creates a Mock, it goes through it's members and sets all overridable to return `Mock<T>.Object`'s

    var root = _moq.Create<Root>();

    root.Tree.Branch.Leaf.Grow();

    _moq.Of<ILeaf>().Verify(x => x.Grow(), Times.Once);

## Lazy Mocking

When Moqqer does recursive overriding, it only overrides the current class, and overrides all of its overridable properties to `Func<T>`'s which return another created `Mock<T>`. This prevents Moqqer from crawling the whole object graph straight away, as more often then not, this is unnecesary.

    var root = _moq.Create<Root>();

    _moq.Mocks.Should().NotContainKey(typeof(IBranch));

    root.Tree.Branch.Leaf.Grow();

    _moq.Mocks.Should().ContainKey(typeof(IBranch));

# Non Mockable Defaults

|Type  |  Defaulted To
|--|--
|Mockable Object | `Mock<T>`
|`String`  |  `String.Empty`
|`List<T>` |  `new List<T>()`

# Installing

Find it on NuGet under MoqInjectionContainer

https://www.nuget.org/packages/MoqInjectionContainer/

# Roadmap

## Quicker Verification:

    _moq.Verify<SomeClass>(x => x.A).Never();
    _moq.Verify<SomeClass>(x => x.A).Once();
    _moq.Verify<SomeClass>(x => x.A).Times(3);
    _moq.Verify<SomeClass>(x => x.A).AtLeast(5);
    _moq.Verify<SomeClass>(x => x.A).AtMost(5);

## Easier Setup

When you don't want to provide `It.IsAny<T>` for methods, you can provide a method name and let Moqqer use reflection.

    // Setup
    _moq.Setup(nameof(_root.Tree)).Returns(new Tree());

    // Verify
    _moq.Verify(nameof(_root.Tree)).Once();
