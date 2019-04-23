using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT3_CookControllerTest
    {
        private IUserInterface _userInterface;
        private IDisplay _displayUut;
        private IDisplay _displayUI;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        private MicrowaveOvenClasses.Controllers.CookController _uut;
        private ILight _light;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;
        private ICookController _cookController;

        [SetUp]
        public void SetUp()
        {
            _output = Substitute.For<IOutput>();
            _displayUut = Substitute.For<IDisplay>();
            _displayUI = Substitute.For<IDisplay>();
            _light = Substitute.For<ILight>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _cookController = Substitute.For<ICookController>();

            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            
            _uut = new MicrowaveOvenClasses.Controllers.CookController(_timer, _displayUut, _powerTube);

            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _displayUI, _light, _cookController);

            _uut.UI = _userInterface;

        }



        [TestCase(3000)]
        [TestCase(1000)]
        public void Ready_DoorOpen_LightOn(int time)
        {
            // This tests that uut is calling CookingIsDone in the UI
            // and the UI calls clear on the display and turns off the light
            // simulating the event through NSubstitute
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _uut.StartCooking(50, time);
            Thread.Sleep((time + 1000));

            // Jeg forstår ikke hvorfor den modtager 2 kald og ikke 1...
            // OG inde i Userinterface, der kalder UI "StartCooking"
            // Fra cookingcontroller. 
            // MEN den kalder den med time (som er 1++ hver gang) ganget med 60
            // Altså den kalder time * 60. 
            // Dvs. ved staartcooking med time  = 60
            // Sættes timeren til 60. Dette giver vel ikke mening
            // Den burde have 1000 i timer parametern og ikke 60
            // For at sætte 1 sek.


            _displayUI.Received(1).Clear();
            _light.Received(1).TurnOff();
        }

        [Test]
        public void OnTimerExpired_CookingIsDone_LightDisplatNotCalled(int time)
        {
            // This tests that uut is calling CookingIsDone in the UI
            // and the UI calls clear on the display and turns off the light
            // simulating the event through NSubstitute
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _uut.StartCooking(50, time);
            _uut.Stop();

            // Jeg forstår ikke hvorfor den modtager 2 kald og ikke 1...
            // OG inde i Userinterface, der kalder UI "StartCooking"
            // Fra cookingcontroller. 
            // MEN den kalder den med time (som er 1++ hver gang) ganget med 60
            // Altså den kalder time * 60. 
            // Dvs. ved staartcooking med time  = 60
            // Sættes timeren til 60. Dette giver vel ikke mening
            // Den burde have 1000 i timer parametern og ikke 60
            // For at sætte 1 sek.


            _displayUI.DidNotReceive().Clear();
            _light.DidNotReceive().TurnOff();
        }

    }
}
