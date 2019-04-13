using MicrowaveOvenClasses.Controllers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT1_CookControllerTest
    {
        private IUserInterface _userInterface;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        private ICookController _uut;

        [SetUp]
        public void SetUp()
        {   
            _userInterface = Substitute.For<IUserInterface>();
            _timer = Substitute.For<ITimer>();
            _output = Substitute.For<IOutput>();
            _display = Substitute.For<IDisplay>();

            _powerTube = new PowerTube(_output);
            _uut = new MicrowaveOvenClasses.Controllers.CookController(_timer, _display, _powerTube, _userInterface);
        }

        #region Powertube

        [TestCase(0)]
        [TestCase(-2)]
        [TestCase(101)]
        public void StartCooking_TurnOn_ThrowsExeption(int power)
        {
            // Er det i orden bare at skrive time sådan??
            int time = 10;
            Assert.That(() => _uut.StartCooking(power, time), Throws.Exception);
        }
        
        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void StartCooking_AlreadyTurnedOn_ThrowsExeption(int power)
        {
            // Er det i orden bare at skrive time sådan??
            int time = 10;
            _uut.StartCooking(power, time);

            Assert.That(() => _uut.StartCooking(power, time), Throws.Exception);
        }


        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        public void StartCooking_CorrectValues_DoesNotThrowExeption(int power)
        {
            // Er det i orden bare at skrive time sådan??
            int time = 10;

            Assert.That(() => _uut.StartCooking(power, time), Throws.Nothing);
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        [TestCase(37)]
        public void StartCooking_CorrectValues_OutputsCorrectPower(int power)
        {
            // Er det i orden bare at skrive time sådan??
            int time = 10;
            _uut.StartCooking(power, time);

            _output.Received(1).OutputLine(Arg.Is<string>(str => 
                str.Contains($"{power}")));
        }

        [TestCase(1)]
        [TestCase(50)]
        [TestCase(100)]
        [TestCase(37)]
        public void Stop_AlreadyTurnedOn_OutputsTurnedOff(int power)
        {
            // Er det i orden bare at skrive time sådan??
            int time = 10;
            _uut.StartCooking(power, time);
            _uut.Stop();

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.Contains($"turned off")));
        }


        [Test]
        public void Stop_NotOn_TurnedOff()
        {
            // Er det i orden bare at skrive time sådan??
            _uut.Stop();

            _output.DidNotReceive().OutputLine(Arg.Any<string>());
        }

        [Test]
        public void Stop_AlreadyStoppedByTimer_TurnOffCalledOnce()
        {
            _uut.StartCooking(50, 50);
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _uut.Stop();

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.Contains($"turned off")));

        }

        [Test]
        public void OnTimerExpired_AlreadyCooking_TurnOff()
        {
            _uut.StartCooking(50, 50);


            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.Contains($"turned off")));
        }

        [Test]
        public void OnTimerExpired_NotCooking_TurnOffNotCalled()
        {
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _output.DidNotReceive().OutputLine(Arg.Any<string>());

        }

        [Test]
        public void OnTimerExpired_CoookingFalse_TurnOffCalledOnce()
        {
            _uut.StartCooking(50, 50);
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            // Call expired again - cooking now false
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.Contains($"turned off")));

        }

        [Test]
        public void OnTimerExpired_AlreadyStopped_TurnOffCalledOnce()
        {
            _uut.StartCooking(50, 50);

            _uut.Stop();
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.Contains($"turned off")));

        }

        #endregion

    }
}
