using System;
using AutoFixture;
using Moq;
using WialonRetranslator.BusinessLogic.Services;
using WialonRetranslator.Core.Interfaces;
using WialonRetranslator.DataAccess.Repository;
using Xunit;

namespace WialonRetranslator.BusinessLogic.Tests
{
    public class UnitTest1
    {
        private readonly Fixture _fixture;
        private readonly Mock<IPointRepository> _membersRepositoryMock;
        private readonly ProtocolParserService _service;
        
        public UnitTest1()
        {
            _fixture = new Fixture();
            _membersRepositoryMock = new Mock<IPointRepository>();
            _service = new ProtocolParserService(_membersRepositoryMock.Object);
        }
        [Fact]
        public void StartTCPServer()
        {
            // arrange
            
            // act
            
            // assert
        }
    }
}