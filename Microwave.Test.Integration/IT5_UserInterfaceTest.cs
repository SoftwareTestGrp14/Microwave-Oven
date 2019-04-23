using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.UserInterface
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
            _uut = new MicrowaveOvenClasses.Controllers.UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _displayFake, _light, _cookController);
            _light = Substitute.For<ILight>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _powerButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _cookController = new MicrowaveOvenClasses.Controllers.CookController(_timer, _displayReal, _powerTube, _uut);
            _displayFake = Substitute.For<IDisplay>();
            _displayReal = new Display(_output);
            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _output = Substitute.For<IOutput>();
        }

        
    }
}