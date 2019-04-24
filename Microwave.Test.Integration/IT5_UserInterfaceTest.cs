using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

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
        private ICookController _cookController;
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
        }

        #region CookController

        
        [TestCase(50)]
        [TestCase(400)]
        [TestCase(700)]
        public void StartCancelBtnPressedTest_WhileSetTime_StartCooking(int powerLevel)
        {
            var powerPercentage = powerLevel/7;

            for (int pow = 0; pow < powerLevel; pow+=50)
            {
                _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            }
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            //As the default power setting is 50, we test if the powertube is outputting the expected power when the button is pressed
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains($"PowerTube works with {powerPercentage} %")));
        }

        [Test]
        public void StartCancelBtnPressedTest_WhileSetTime_StartCooking_CorrectTiming()
        {
            //First we need to enter the state COOKING
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Default time is 1 minute.
            Thread.Sleep(1000);
            

            Assert.That(_timer.TimeRemaining,Is.EqualTo(59));
        }

        [Test]
        public void StartCancelBtnPressedTest_WhileCooking_StopCooking()
        {
            //First we need to enter the state COOKING
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Should stop the cooking
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube turned off")));
        }

        [Test]
        public void DoorOpenedTest_WhileCooking_StopCooking()
        {
            //First we need to enter the state COOKING
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //shoulf stop the cooking
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube turned off")));
        }

        #endregion
    }
}