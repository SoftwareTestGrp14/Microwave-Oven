﻿using MicrowaveOvenClasses.Controllers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT4_CookControllerTest
    {
        private IUserInterface _userInterface;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IOutput _output;
        private ILight _light;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;
        private ICookController _uut;

        [SetUp]
        public void SetUp()
        {
            _output = Substitute.For<IOutput>();
            _light = Substitute.For<ILight>();
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();

            _userInterface = new MicrowaveOvenClasses.Controllers.UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _uut);
            _timer = new Timer();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _uut = new MicrowaveOvenClasses.Controllers.CookController(_timer, _display, _powerTube, _userInterface);
        }

        #region Display


        [TestCase(1000)]
        [TestCase(2000)]
        [TestCase(4000)]
        [TestCase(10000)]
        public void StartCooking_TurnOn_OutputShowsSeconds(int time)
        {
            int power = 50;
            _uut.StartCooking(power, time);
            Thread.Sleep(time + 1000);

            int min = (_timer.TimeRemaining / 1000) / 60;
            int sec = _timer.TimeRemaining / 1000;
            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Contains($"Display shows: {min:D2}:{sec:D2}")));
        }
   

        #endregion

    }
}
