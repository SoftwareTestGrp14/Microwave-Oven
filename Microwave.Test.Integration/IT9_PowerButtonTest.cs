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

namespace Microwave.Test.Unit
{
    class IT9_PowerButtonTest
    {
        private ILight _light;
        private IButton _timerBtn;
        private IButton _startCancelBtn;
        private IButton _uut;
        private IDoor _door;
        private ICookController _cookController;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        private IUserInterface _userInterface;

        [SetUp]
        public void SetUp()
        {
            _timerBtn = Substitute.For<IButton>();
            _startCancelBtn = Substitute.For<IButton>();
            _uut = new Button();
            _output = Substitute.For<IOutput>();
            _door = new Door();
            _light = new Light(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new MicrowaveOvenClasses.Controllers.UserInterface(_uut, _timerBtn, _startCancelBtn, _door, _display, _light, _cookController);
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
                _uut.Press();
            }

            _output.Received(receivedTimes).OutputLine(Arg.Is<string>(str => str.Contains(($"Display shows: {power} W"))));
        }
    }
}
