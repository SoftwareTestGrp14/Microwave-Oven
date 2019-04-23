using MicrowaveOvenClasses.Controllers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Interfaces;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT2_CookControllerTest
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
            _userInterface = Substitute.For<IUserInterface>();
            _output = Substitute.For<IOutput>();
            _display = Substitute.For<IDisplay>();

            _timer = new Timer();
            _powerTube = new PowerTube(_output);
            _uut = new MicrowaveOvenClasses.Controllers.CookController(_timer, _display, _powerTube, _userInterface);
        }

        // Man kan ikke rigtige integrationsteste timeren medd CookController som topmodul
        // Uden at bruge timeren som topmodul samtidig med CookController. 
        // Dvs. at cookcontrolleren er afhængig af timerens event
        // Så der ved integrationstestes den bidirektionelle association mellem CK og Timeren 
        // i den samme integrationstest. 
        // Integrationstesten vil ikke virke hvis den ene af de 2 fejler

        #region Timer <> CookController

        [TestCase(0)]
        [TestCase(-2000)]
        [TestCase(5000)]
        [TestCase(99999)]
        public void StartCooking_StartTimer_RemainingTime(int time)
        {
            _uut.StartCooking(50, time);
            

            Assert.That(_timer.TimeRemaining, Is.EqualTo(time));
            // Man burde ikke at kunne sætte timeren til at være negativ tid
        }

        [TestCase(3000)]
        [TestCase(1000)]
        [TestCase(10000)]
        public void StartCooking_TimerEnabled_TimerStartedAndExpired(int time)
        {

            _uut.StartCooking(50, time);

            // For at tjekke at timeren er enabled skal man lade den køre 
            // Og se at der bliver lavet kald til display.Showtime hver tick

            

            _userInterface.Received(1).CookingIsDone();
            _display.Received(time/1000).ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }


        [TestCase(5000)]
        [TestCase(2500)]
        [TestCase(10000)]
        public void Stop_TimerStopped_TimerDisabled(int time)
        {
            _uut.StartCooking(50, time);

            // For at tjekke at den bliver disabled skal man her kalde startcooking
            // Trykke stop
            // Vente i lang tid
            // Asserte på at timerexpired ikke bliver kaldt
            // Må være lig med at den er disabled

            // DidNotReceive CookingIsDone tells us that the timer 
            // did not expire. 

            // Display received ShowTime tells us, that the timer started
            // By calling StartCooking

            // By calling stop, the timer got disabled
            // It is verified because "Expire" was not called

            Thread.Sleep(time/2);
            _uut.Stop();

            Thread.Sleep(time / 2);

            _userInterface.DidNotReceive().CookingIsDone();
            _display.Received().ShowTime(Arg.Any<int>(), Arg.Any<int>());
            
        }

        
        [TestCase(3000)]
        [TestCase(2500)]
        [TestCase(5000)]
        public void StopStart_TimerStoppedAndStarted_TimerDisabledEnabled(int time)
        {
            _uut.StartCooking(50, time);


            Thread.Sleep(time / 2);
            _uut.Stop();
            Thread.Sleep(time / 2);
            _uut.StartCooking(50, _timer.TimeRemaining);
            Thread.Sleep(time);

            _userInterface.Received(1).CookingIsDone();
            _display.Received().ShowTime(Arg.Any<int>(), Arg.Any<int>());

        }

        #endregion


    }
}
