using System;
using FluentAssertions;
using Moq;
using MoqqerNamespace.Extensions;
using MoqqerNamespace.Tests.TestClasses;
using NUnit.Framework;

namespace MoqqerNamespace.Tests.MoqExtensions
{
    [TestFixture]
    public class MoqFluentVerifyBuilderTests
    {
        private Moqqer _moq;
        private Root _root;

        [SetUp]
        public void A_TestSetup()
        {
            _moq = new Moqqer();
            _root = _moq.Create<Root>();
        }

        [Test]
        public void Verify_Once_MethodNotCalled_Exception()
        {
            Action action = () => _moq.Verify<ILeaf>(x => x.Grow()).Once();

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock once, but was 0 times*");
        }

        [Test]
        public void Verify_Once_MethodCalledOnce_Success()
        {
            _root.Water();

            _moq.Verify<ILeaf>(x => x.Grow()).Once();
        }

        [Test]
        public void Verify_Once_MethodCalledTwice_Exception()
        {
            _root.Water();
            _root.Water();

            Action action = () => _moq.Verify<ILeaf>(x=> x.Grow()).Once();

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock once, but was 2 times*");
        }


        [Test]
        public void Verify_WasCalled_MethodNotCalled_Exception()
        {
            Action action = () => _moq.Verify<ILeaf>(x => x.Grow()).WasCalled();

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock at least once, but was never performed*");
        }

        [Test]
        public void Verify_WasCalled_MethodCalledOnce_Success()
        {
            _root.Water();

            _moq.Verify<ILeaf>(x => x.Grow()).WasCalled();
        }

        [Test]
        public void Verify_WasCalled_MethodCalledTwice_Success()
        {
            _root.Water();
            _root.Water();

            _moq.Verify<ILeaf>(x => x.Grow()).WasCalled();
        }


        [Test]
        public void Verify_Times3_MethodNotCalled_Exception()
        {
            Action action = () => _moq.Verify<ILeaf>(x => x.Grow()).Times(3);

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock exactly 3 times, but was 0 times*");
        }

        [Test]
        public void Verify_Times3_MethodCalled3Times_Success()
        {
            _root.Water();
            _root.Water();
            _root.Water();

            _moq.Verify<ILeaf>(x => x.Grow()).Times(3);
        }

        [Test]
        public void Verify_Times3_MethodCalledTwice_Exception()
        {
            _root.Water();
            _root.Water();

            Action action = () => _moq.Verify<ILeaf>(x => x.Grow()).Times(3);

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock exactly 3 times, but was 2 times*");
        }

        [Test]
        public void Verify_Times3_MethodCalled4Times_Exception()
        {
            _root.Water();
            _root.Water();
            _root.Water();
            _root.Water();

            Action action = () => _moq.Verify<ILeaf>(x => x.Grow()).Times(3);

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock exactly 3 times, but was 4 times*");
        }

        [Test]
        public void Verify_Never_MethodNotCalled_Success()
        {
            _moq.Verify<ILeaf>(x => x.Grow()).Never();
        }

        [Test]
        public void Verify_Never_MethodCalledOnce_Exception()
        {
            _root.Water();

            Action action = () => _moq.Verify<ILeaf>(x => x.Grow()).Never();

            action.ShouldThrow<MockException>()
                .WithMessage("*Expected invocation on the mock should never have been performed, but was 1 times*");
        }



        [Test]
        public void Verify_TimesOnce_MethodCalledOnce_Success()
        {
            _root.Water();

            _moq.Verify<ILeaf>(x => x.Grow()).Times(Times.Once());
        }
    }
}