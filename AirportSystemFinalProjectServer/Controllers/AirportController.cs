using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using Models;
using Services;
using Interfaces;
using System.Timers;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private IRepository _repository;
        private IAirport _airport;
        private Timer _aTimer;

        public AirportController(IRepository repository, IAirport airport)
        {
            _airport = airport;
            _repository = repository;
            _aTimer = new Timer();
        }

        [HttpGet]
        public void Get()
        {
            //RemoveGarbagePlanes();
            //_aTimer.Interval = 5000;
            //_aTimer.Elapsed += OnTimedEvent;
            //_aTimer.AutoReset = true;
            //_aTimer.Enabled = true;
            for (int i = 0; i < 30; i++)
            {
                _airport.Start();
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _airport.Start();
        }

        private void RemoveGarbagePlanes()
        {
            _repository.RemoveGarbagePlanes();
        }
    }
}
