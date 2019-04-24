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
    class IT11_StartCancelButtonTest
    {
        private IUserInterface _userInterface;
        private ILight _light;
        private IButton _timeButton;
        private IButton _uut;
        private IButton _powerButton;
        private IDoor _door;
        private ICookController _cookController;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;

        [SetUp]
        public void SetUp()
        {
            _output = Substitute.For<IOutput>();

            _light = new Light(_output);
            _timeButton = new Button();
            _powerButton = new Button();
            _door = new Door();
            _display = new Display(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _uut = new Button();

            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _uut, _door, _display, _light, _cookController);
        }

        [Test]
        public void StartCancelBtnPressedTest_WhileSetTime_StartCooking()
        {
            //First we need to enter the state SETTIME
            _powerButton.Press();
            _timeButton.Press();

            //As the default power setting is 50, we test if the powertube is outputting the expected power when the button is pressed
            _uut.Press();

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube works with 7 %")));
        }

        
        [Test]
        public void StartCancelBtnPressedTest_WhileCooking_StopCooking()
        {
            //First we need to enter the state COOKING
            _powerButton.Press();
            _timeButton.Press();

            //Should stop the cooking
            _uut.Press();
            _uut.Press();

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube turned off")));
        }
    }
}
