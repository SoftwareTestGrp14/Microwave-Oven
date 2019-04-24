using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT5_UserInterfaceTest
    {
        private MicrowaveOvenClasses.Controllers.UserInterface _uut;
        private ILight _light;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IButton _powerButton;
        private IDoor _door;
        private MicrowaveOvenClasses.Controllers.CookController _cookController; //Må vi overhovedet det her!?!?!?
        private IDisplay _displayFake;
        private IDisplay _displayReal;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        
        [SetUp]
        public void SetUp()
        {
            
            _light = Substitute.For<ILight>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _powerButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _displayFake = Substitute.For<IDisplay>();
            _output = Substitute.For<IOutput>();

            _displayReal = new Display(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);

            _cookController = new MicrowaveOvenClasses.Controllers.CookController(_timer, _displayReal, _powerTube);
            _uut = new MicrowaveOvenClasses.Controllers.UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _displayFake, _light, _cookController);
            _cookController.UI = _uut;
        }

        #region CookController

        [Test]
        public void StartCancelBtnPressedTest_WhileSetTime_StartCooking()
        {
            //First we need to enter the state SETTIME
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            //As the default power setting is 50, we test if the powertube is outputting the expected power when the button is pressed
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube works with 50 %")));
        }

        [Test]
        public void StartCancelBtnPressedTest_WhileCooking_StopCooking()
        {
            //First we need to enter the state SETTIME
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //As the default power setting is 50, we test if the powertube is outputting the expected power when the button is pressed
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube turned off")));
        }

        [Test]
        public void DoorOpenedTest_WhileCooking_StopCooking()
        {
            //First we need to enter the state SETTIME
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //As the default power setting is 50, we test if the powertube is outputting the expected power when the button is pressed
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube turned off")));
        }

        #endregion
    }
}