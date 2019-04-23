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
    class IT7_UserInterfaceTest
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
            _timerBtn = Substitute.For<IButton>();
            _startCancelBtn = Substitute.For<IButton>();
            _powerBtn = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _output = Substitute.For<IOutput>();
            _light = new Light(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
            _uut = new MicrowaveOvenClasses.Controllers.UserInterface(_powerBtn, _timerBtn, _startCancelBtn, _door, _display, _light, _cookController);
        }

        [Test]
        public void DoorOpenCalled_UserInterfaceStateREADY_LightTurnOn()
        {
            //State is already READY

            //Trigger the event
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned on"))));
        }

        [Test]
        public void DoorOpenCalled_UserInterfaceStateSETPOWER_LightTurnOn()
        {
            //Setting the state
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned on"))));
        }

        [Test]
        public void DoorOpenCalled_UserInterfaceStateSETTIME_LightTurnOn()
        {
            //Setting the state
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned on"))));
        }

        [Test]
        public void DoorOpenCalled_UserInterfaceStateCOOKING_LightTurnOn()
        {
            //Setting the state
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned on"))));

            //This one gets called only once, because the light turns on, when the startCancelBtn is pressed, but when the door is
            //opened, the light can't be turned on again.
        }

        [Test]
        public void DoorClosedCalled_UserInterfaceStateDOOROPEN_LightTurnOff()
        {
            //Setting the state
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _door.Closed += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned off"))));
        }

        [Test]
        public void StartCancelBtnPressed_UserInterfaceStateSETTIME_LightTurnOn()
        {
            //Setting the state
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _startCancelBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned on"))));
        }

        [Test]
        public void CookingIsDoneCalled_LightTurnOff()
        {
            //Setting the state
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _uut.CookingIsDone();

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned off"))));
        }

        [Test]
        public void StartCancelBtnPressed_UserInterfaceStateCOOKING_LightTurnOff()
        {
            //Setting the state
            _powerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timerBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Trigger the event
            _startCancelBtn.Pressed += Raise.EventWith(this, EventArgs.Empty);

            //Check if the light gets turned on
            _output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains(("Light is turned off"))));
        }

    }
}
