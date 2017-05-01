using Dragino.Radio;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Dragino.Lora.Tests
{
    [TestClass]
    public class RegisterManagerTests
    {
        private RegisterManager _registerManager;
        private MockSpiDevice _mockSpiDevice;

        [Register(0x42, 1)]
        private class MockRegister : Register
        {
            public MockRegister(byte address, byte length, byte[] registerValues)
                : base(address, length)
            {
                RegisterValues = registerValues;
            }

            public MockRegister(byte[] registerValues)
                : this(0x42, 1, registerValues)
            {
            }

            public byte[] RegisterValues { get; }

            internal override byte[] GetAsBytes()
            {
                return RegisterValues;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mockSpiDevice = new MockSpiDevice();
            var mockSpiComm = new MockSpiComm(_mockSpiDevice);
            _registerManager = new RegisterManager(mockSpiComm);
        }

        [TestMethod]
        public void Read_one_byte_register()
        {
            // ARRANGE
            _mockSpiDevice.WriteAction = buffer =>
            {
                Assert.AreEqual(1, buffer.Length);
                Assert.AreEqual(0x42, buffer[0]);
            };

            _mockSpiDevice.ReadAction = buffer =>
            {
                Assert.AreEqual(1, _mockSpiDevice.WriteActionCount, "The Write must come before the Read");
                Assert.AreEqual(1, buffer.Length);
                buffer[0] = 0x99;
            };

            // ACT
            MockRegister readRegister = _registerManager.Read<MockRegister>();

            // ASSERT
            Assert.AreEqual(1, _mockSpiDevice.ReadActionCount);
            Assert.AreEqual(1, _mockSpiDevice.WriteActionCount);

            Assert.AreEqual(1, readRegister.RegisterValues.Length);
            Assert.AreEqual(0x99, readRegister.RegisterValues[0]);
        }

        [TestMethod]
        public void Read_three_bytes_register()
        {
            // ARRANGE
            _mockSpiDevice.WriteAction = buffer =>
            {
                Assert.AreEqual(1, buffer.Length);
                switch (_mockSpiDevice.WriteActionCount)
                {
                    case 0:
                        Assert.AreEqual(0x42, buffer[0]);
                        break;
                    case 1:
                        Assert.AreEqual(0x43, buffer[0]);
                        break;
                    case 2:
                        Assert.AreEqual(0x44, buffer[0]);
                        break;
                    default:
                        Assert.Fail("Too many writes!");
                        break;
                }
            };

            _mockSpiDevice.ReadAction = buffer =>
            {
                Assert.AreEqual(_mockSpiDevice.ReadActionCount + 1, _mockSpiDevice.WriteActionCount, "The Write must come before the Read");
                Assert.AreEqual(1, buffer.Length);
                switch (_mockSpiDevice.ReadActionCount)
                {
                    case 0:
                        buffer[0] = 0x12;
                        break;
                    case 1:
                        buffer[0] = 0x34;
                        break;
                    case 2:
                        buffer[0] = 0x56;
                        break;
                    default:
                        Assert.Fail("Too many reads!");
                        break;
                }
            };

            // ACT
            MockRegister readRegister = _registerManager.Read<MockRegister>();

            // ASSERT
            Assert.AreEqual(3, _mockSpiDevice.ReadActionCount);
            Assert.AreEqual(3, _mockSpiDevice.WriteActionCount);

            Assert.AreEqual(3, readRegister.RegisterValues.Length);
            Assert.AreEqual(0x12, readRegister.RegisterValues[0]);
            Assert.AreEqual(0x34, readRegister.RegisterValues[1]);
            Assert.AreEqual(0x56, readRegister.RegisterValues[2]);
        }

        [TestMethod]
        public void Write_one_byte_register()
        {
            // ARRANGE
            var mockRegister = new MockRegister(0x42, 1, new byte[] { 0x99 });

            _mockSpiDevice.WriteAction = buffer =>
            {
                Assert.AreEqual(2, buffer.Length);
                Assert.AreEqual(0x42 | 0x80, buffer[0]);
                Assert.AreEqual(0x99, buffer[1]);
            };

            // ACT
            _registerManager.Write(mockRegister);

            // ASSERT
            Assert.AreEqual(0, _mockSpiDevice.ReadActionCount);
            Assert.AreEqual(1, _mockSpiDevice.WriteActionCount);
        }

        [TestMethod]
        public void Write_three_bytes_register()
        {
            // ARRANGE
            var mockRegister = new MockRegister(0x42, 1, new byte[] { 0x12, 0x34, 0x56 });

            _mockSpiDevice.WriteAction = buffer =>
            {
                Assert.AreEqual(2, buffer.Length);

                switch (_mockSpiDevice.WriteActionCount)
                {
                    case 0:
                        Assert.AreEqual(0x42 | 0x80, buffer[0]);
                        Assert.AreEqual(0x12, buffer[1]);
                        break;

                    case 1:
                        Assert.AreEqual(0x43 | 0x80, buffer[0]);
                        Assert.AreEqual(0x34, buffer[1]);
                        break;

                    case 2:
                        Assert.AreEqual(0x44 | 0x80, buffer[0]);
                        Assert.AreEqual(0x56, buffer[1]);
                        break;

                    default:
                        Assert.Fail("Too many writes!");
                        break;
                }
            };

            // ACT
            _registerManager.Write(mockRegister);

            // ASSERT
            Assert.AreEqual(0, _mockSpiDevice.ReadActionCount);
            Assert.AreEqual(3, _mockSpiDevice.WriteActionCount);
        }
    }
}