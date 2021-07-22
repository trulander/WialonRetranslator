using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WialonRetranslator.DataAccess.Entities;

namespace WialonRetranslator.DataAccess.Repository
{
    public class PointRepository : IPointRepository
    {
        private readonly WialonDbContext _dbContext;

        public PointRepository(WialonDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create(Core.Models.Point point)
        {
            _dbContext.Points.Add(new Point()
            {
                PointTime = point.PointTime,
                DeviceId = point.DeviceId,
                Latitude = point.Latitude,
                Longtitude = point.Longtitude
            });
            _dbContext.SaveChanges();
        }


        public Core.Models.Point Read(int pointId)
        {
            var message = _dbContext.Points.FirstOrDefault(x => x.PointId == pointId);
            return new Core.Models.Point()
            {
                PointId = message.PointId,
                PointTime = message.PointTime,
                DeviceId = message.DeviceId,
                Latitude = message.Latitude,
                Longtitude = message.Longtitude
            };
        }

        public Core.Models.Point[] Read()
        {
            var messages = _dbContext.Points.OrderBy(x => x.PointTime).Take(10);
            var result = new List<Core.Models.Point>();
            foreach (var message in messages)
            {
                result.Add(new Core.Models.Point()
                {
                    PointId = message.PointId,
                    PointTime = message.PointTime,
                    DeviceId = message.DeviceId,
                    Latitude = message.Latitude,
                    Longtitude = message.Longtitude
                });
            }
            return result.ToArray();
        }

        public void Delete(int pointId)
        {
            _dbContext.Points.Remove(new Point()
            {
                PointId = pointId
            });
        }
    }
}