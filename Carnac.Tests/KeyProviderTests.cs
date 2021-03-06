using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Carnac.Logic;
using Carnac.Logic.KeyMonitor;
using Microsoft.Win32;
using NSubstitute;
using Xunit;

namespace Carnac.Tests
{
    public class KeyProviderTests
    {
        readonly IPasswordModeService passwordModeService;
        readonly IDesktopLockEventService desktopLockEventService;

        public KeyProviderTests()
        {
            passwordModeService = new PasswordModeService();
            desktopLockEventService = Substitute.For<IDesktopLockEventService>();
            desktopLockEventService.GetSessionSwitchStream().Returns(Observable.Never<SessionSwitchEventArgs>());
        }

        [Fact]
        public async Task ctrlshiftl_is_processed_correctly()
        {
            // arrange
            var player = KeyStreams.CtrlShiftL();
            var provider = new KeyProvider(player, passwordModeService, desktopLockEventService);

            // act
            var processedKeys = await provider.GetKeyStream().ToList();

            // assert
            Assert.Equal(new[] { "Ctrl", "Shift", "L" }, processedKeys.Single().Input);
        }

        [Fact]
        public async Task shift_is_not_outputted_when_is_being_used_as_a_modifier_key()
        {
            // arrange
            var player = KeyStreams.ShiftL();
            var provider = new KeyProvider(player, passwordModeService, desktopLockEventService);

            // act
            var processedKeys = await provider.GetKeyStream().ToList();

            // assert

            Assert.Equal(new[] { "L" }, processedKeys.Single().Input);
        }

        [Fact]
        public async Task key_without_shift_is_lowercase()
        {
            // arrange
            var player = KeyStreams.LetterL();
            var provider = new KeyProvider(player, passwordModeService, desktopLockEventService);

            // act
            var processedKeys = await provider.GetKeyStream().ToList();

            // assert
            Assert.Equal(new[] { "l" }, processedKeys.Single().Input);
        }

        [Fact]
        public async Task verify_number()
        {
            // arrange
            var player = KeyStreams.Number1();
            var provider = new KeyProvider(player, passwordModeService, desktopLockEventService);

            // act
            var processedKeys = await provider.GetKeyStream().ToList();

            // assert
            Assert.Equal(new[] { "1" }, processedKeys.Single().Input);
        }

        [Fact]
        public async Task verify_shift_number()
        {
            // arrange
            var player = KeyStreams.ExclaimationMark();
            var provider = new KeyProvider(player, passwordModeService, desktopLockEventService);

            // act
            var processedKeys = await provider.GetKeyStream().ToList();

            // assert
            Assert.Equal(new[] { "!" }, processedKeys.Single().Input);
        }

        [Fact]
        public async Task keyprovider_detects_windows_key_presses()
        {
            // arrange
            var player = KeyStreams.WinkeyE();
            var provider = new KeyProvider(player, passwordModeService, desktopLockEventService);

            // act
            var processedKeys = await provider.GetKeyStream().ToList();

            // assert
            Assert.Equal(new[] { "Win", "e" }, processedKeys.Single().Input);
        }
    }
}