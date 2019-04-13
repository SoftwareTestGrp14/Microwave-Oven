using System;
using System.Collections.Generic;
using System.Text;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.CookController
{
    [TestFixture]
    public class IT3_CookControllerTest
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
            _output = Substitute.For<IOutput>();
            _display = Substitute.For<IDisplay>();

            
            _timer = new Timer();
            _powerTube = new PowerTube(_output);

            //_userInterface = new UserInterface();
            _uut = new MicrowaveOvenClasses.Controllers.CookController(_timer, _display, _powerTube, _userInterface);
        }

    }
}
