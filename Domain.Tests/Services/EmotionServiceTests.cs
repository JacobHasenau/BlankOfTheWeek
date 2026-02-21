using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Domain.Models;
using Domain.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Domain.Tests.Services
{
    [TestFixture]
    public class GivenAnEmotionService
    {
        private IFixture _fixture;
        private IEmotionRepoistory _fakeEmotionRepo;
        private IDefiner _fakeDefiner;
        private EmotionService _service;

        [OneTimeSetUp]
        public void CreateFixture()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
        }

        [SetUp]
        public void CreateService()
        {
            _fakeEmotionRepo = _fixture.Freeze<IEmotionRepoistory>();
            _fakeDefiner = _fixture.Freeze<IDefiner>();

            _service = _fixture.Create<EmotionService>();
        }

        [TestFixture]
        private abstract class WhenGetRandomEmotionIsCalled : GivenAnEmotionService
        {
            private IEmotionState _fakeReturnedEmotionsState;
            private Emotion _returnedEmotion;
            private List<Emotion> _emotions;
            private DefinedWord _emotionDefinition;

            [SetUp]
            public void CreateRandomEmotions()
            {
                _fakeReturnedEmotionsState = _fixture.Create<IEmotionState>();
                _fakeReturnedEmotionsState.Description.ReturnsNull();
                _returnedEmotion = new Emotion(_fakeReturnedEmotionsState);

                _emotions = [_returnedEmotion];
                _emotionDefinition = _fixture.Create<DefinedWord>();

                _fakeEmotionRepo.GetAll().ReturnsForAnyArgs(_emotions);
                _fakeDefiner.DefineWord(Arg.Is<string>(str => str == _returnedEmotion.Name))
                    .Returns(x => Task.FromResult(_emotionDefinition));
            }

            [TestFixture]
            private class AndRepoReturnsEmotionWithDescription : WhenGetRandomEmotionIsCalled
            {
                private string _expectedDescription { get; set; }

                [SetUp]
                public void ForceDescriptionReturn()
                {
                    _expectedDescription = _fixture.Create<string>();
                    _fakeReturnedEmotionsState.Description.Returns(_expectedDescription);
                }

                [Test]
                public async Task ThenEmotionWithExpectedDescriptionIsReturned()
                {
                    var emotion = await _service.GetRandomEmotion();
                    using (Assert.EnterMultipleScope())
                    {
                        Assert.That(emotion, Is.EqualTo(_returnedEmotion));
                        Assert.That(emotion.Description, Is.EqualTo(_expectedDescription));
                    }
                }
            }

            [TestFixture]
            private class AndRepoReturnsEmotionsWithoutDescription : WhenGetRandomEmotionIsCalled
            {
                [Test]
                public async Task ThenCallsNewDescriptionFound()
                {
                    _ = await _service.GetRandomEmotion();
                    _fakeReturnedEmotionsState.Received(1).DescriptionFound(_emotionDefinition.Meanings.First().Definition, 0);
                }

                [Test]
                public async Task ThenCallsSaveChanges()
                {
                    _ = await _service.GetRandomEmotion();
                    _fakeEmotionRepo.Received(1).SaveChanges(); //TODO: Does this show an error on a computer with the analyzers
                }
            }

        }
    }
}
