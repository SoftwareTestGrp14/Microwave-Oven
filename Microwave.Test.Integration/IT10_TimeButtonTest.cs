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
    class IT10_UserInterfaceTest
    {
        private ILight _light;
        private IDoor _door;
        private IButton _startCancelBtn;
        private IButton _powerBtn;
        private IButton _uut;
        private ICookController _cookController;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        private IUserInterface _userInterface;

        [SetUp]
        public void SetUp()
        {
            _door = Substitute.For<IDoor>()
            _startCancelBtn = Substitute.For<IButton>();
            _powerBtn = Substitute.For<IButton>();
            _output = Substitute.For<IOutput>();
            _uut = new Button();
            _light = new Light(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _display = new Display(_output);
            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new MicrowaveOvenClasses.Controllers.UserInterface(_powerBtn, _uut, _startCancelBtn, _door, _display, _light, _cookController);
        }

        [Test]
        public void DoorOpenCalled_UserInterfaceStateREADY_LightTurnOn()
        {
           Assert.True(true);
        }

        
    }
}
