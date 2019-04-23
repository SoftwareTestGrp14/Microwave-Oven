using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    class IT6_UserInterfaceTest
    {
        private ILight _light;
        private IButton _timerBtn;
        private IButton _startCancelBtn;
        private IButton _powerBtn;
        private IDoor _door;
        private ICookController _cookController;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        private IUserInterface _uut;

        [SetUp]
        public void SetUp()
        {
            _light = Substitute.For<ILight>();
            _timerBtn = Substitute.For<IButton>();
            _startCancelBtn = Substitute.For<IButton>();
            _powerBtn = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _output = Substitute.For<IOutput>();
            _timer=new Timer();
            _powerTube=new PowerTube(_output);
            _display = new Display(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
            _uut=new MicrowaveOvenClasses.Controllers.UserInterface(_powerBtn, _timerBtn, _startCancelBtn, _door, _display, _light, _cookController);
        }

        [TestCase(1, 50, 1)]
        [TestCase(2, 100, 1)]
        [TestCase(3, 150, 1)]
        [TestCase(4, 200, 1)]
        [TestCase(14, 700, 1)]
        [TestCase(15, 50, 2)]
        public void PowerBtnPressedTest_DisplayShowingCurrentPower(int timesPressed, int power, int receivedTimes)
        {
            for (int i = 0; i < timesPressed; i++)
            {
                _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }
            
            _output.Received(receivedTimes).OutputLine(Arg.Is<string>(str=>str.Contains(($"Display shows: {power} W"))));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(14, 14)]
        [TestCase(100, 100)]
        public void TimerBtnPressedTest_DisplayShowingCurrentTime(int timesPressed, int min)
        {
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            for (int i = 0; i < timesPressed; i++)
            {
                _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }

            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains($"Display shows: {min:D2}:00")));

        }

        [Test]
        public void CookingIsDoneCalled_DisplayCleared()
        {
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _uut.CookingIsDone();

            //Is received twice because the startCancelBtn.Pressed event calls display.clear, and the CookingIsDone calls display.clear
            _output.Received(2).OutputLine(Arg.Is<string>(str => str.Contains($"Display cleared")));
        }
    }
}
