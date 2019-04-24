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
            _door = new Door();
            _startCancelBtn = Substitute.For<IButton>();
            _powerBtn = new Button();
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
        public void TimeButtonPressed_OutPutLineCalledWith2Min()
        {
            _powerBtn.Press();
            _uut.Press();
            _uut.Press();

            _output.Received().OutputLine("Display shows: 02:00");
        }

        [TestCase(5)]
        [TestCase(1)]
        [TestCase(10)]
        public void TimeButtonPressed_NumberOfTimes_OutputLineCalledNumTimes(int num)
        {
            _powerBtn.Press();

            for (int i = 0; i < num; i++)
            {
                _uut.Press();
            }

            _output.Received(num+1).OutputLine(Arg.Any<string>());
        }

    }
}
