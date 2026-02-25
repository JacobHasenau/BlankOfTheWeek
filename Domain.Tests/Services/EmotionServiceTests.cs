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
    public abstract class GivenAnEmotionService
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

        [TearDown]
        public void ClearCalls()
        {
            _fakeEmotionRepo.ClearReceivedCalls();
            _fakeDefiner.ClearReceivedCalls();
        }

        [TestFixture]
        private abstract class WhenGetRandomEmotionIsCalled : GivenAnEmotionService
        {
            private IEmotionState _fakeReturnedEmotionsState;
            private Emotion _returnedEmotion;
            private List<Emotion> _emotions;
            private DefinedWord _emotionDefinition;
            private string _emotionName;

            [SetUp]
            public void CreateRandomEmotions()
            {
                _emotionName = _fixture.Create<string>();
                _fakeReturnedEmotionsState = _fixture.Create<IEmotionState>();
                _fakeReturnedEmotionsState.Description.ReturnsNull();
                _fakeReturnedEmotionsState.Name.Returns(_emotionName);
                _returnedEmotion = new Emotion(_fakeReturnedEmotionsState);

                _emotions = [_returnedEmotion];
                _emotionDefinition = _fixture.Create<DefinedWord>();

                _fakeEmotionRepo.GetAll().ReturnsForAnyArgs(x => _emotions);

                var definerTask = Task.FromResult(_emotionDefinition);
                _fakeDefiner.DefineWord(Arg.Is<string>(str => str == _emotionName))
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
                public async Task ThenCallsSaveChangesAtLeastOnce()
                {
                    _ = await _service.GetRandomEmotion();
                    _fakeEmotionRepo.Received(1).SaveChanges(); //TODO: Does this show an error on a computer with the analyzers
               }
            }

            [TestFixture]
            private class AndRepoReturnsNoEmotions : WhenGetRandomEmotionIsCalled 
            {
                [SetUp]
                public void ClearEmotions()
                {
                    _emotions = [];
                }

                [Test]
                public async Task ThenNullReturned()
                {
                    var result = await _service.GetRandomEmotion();
                    Assert.That(result, Is.Null);
                }
            }
        }

        [TestFixture]
        private abstract class WhenAddEmotionIsCalled : GivenAnEmotionService
        {
            private DefinedWord? _fetchedDefinition;
            private Emotion? _fetchedEmotion;
            private string _emotionName;
            private string? _emotionDescription;

            [SetUp]
            public void SetupRepoToNotReturnEmotionAndDefinerToReturnDefintion()
            {
                _fetchedEmotion = null;
                _emotionName = _fixture.Create<string>();
                _fetchedDefinition = _fixture.Create<DefinedWord>();

                _fakeEmotionRepo.GetByName(Arg.Is<string>(str => str == _emotionName))
                    .Returns(x => Task.FromResult(_fetchedEmotion));

                _fakeEmotionRepo.Add(Arg.Any<Emotion>())
                    .Returns(callInfo => callInfo.Arg<Emotion>());

                _fakeDefiner.DefineWord(Arg.Is<string>(s => s == _emotionName))
                    .Returns(x => Task.FromResult(_fetchedDefinition));

                _emotionName = _fixture.Create<string>();
                _emotionDescription = null;
            }

            [TestFixture]
            private abstract class AndNameIsInvalid : WhenAddEmotionIsCalled
            {
                [Test]
                public async Task ThenInvalidNameResultIsReturned()
                {
                    var result = await _service.AddEmotion(_emotionName, _emotionDescription);
                    Assert.That(result, Is.EqualTo(EmotionAdditionResult.InvalidName));
                }

                [Test]
                public async Task ThenRepositoryIsNotUsed()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.DidNotReceive().GetByName(Arg.Any<string>());
                    _fakeEmotionRepo.DidNotReceive().Add(Arg.Any<Emotion>());
                    _fakeEmotionRepo.DidNotReceive().SaveChanges();
                }

                [Test]
                public async Task ThenDefinerIsNotCalled()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeDefiner.DidNotReceive().DefineWord(Arg.Any<string>());
                }


                [TestFixture]
                private class BecauseNameIsNull : AndNameIsInvalid
                {
                    [SetUp]
                    public void NullOutName()
                    {
                        _emotionName = null!;
                    }
                }

                [TestFixture]
                private class BecauseNameIsEmpty : AndNameIsInvalid
                {
                    [SetUp]
                    public void EmptyName()
                    {
                        _emotionName = string.Empty;
                    }
                }

                [TestFixture]
                private class BecauseNameIsSpace : AndNameIsInvalid
                {
                    [SetUp]
                    public void SpaceName()
                    {
                        _emotionName = " ";
                    }
                }

                [TestFixture]
                private class BecauseNameIsTab : AndNameIsInvalid
                {
                    [SetUp]
                    public void TabName()
                    {
                        _emotionName = "\t";
                    }
                }

                [TestFixture]
                private class BecauseNameIsNewLine : AndNameIsInvalid
                {
                    [SetUp]
                    public void NewLine()
                    {
                        _emotionName = "\n";
                    }
                }

                [TestFixture]
                private class BecauseNameIsReturn : AndNameIsInvalid
                {
                    [SetUp]
                    public async Task ReturnName()
                    {
                        _emotionName = "\r";
                    }
                }
            }

            [TestFixture]
            private class AndEmotionAlreadyExists : WhenAddEmotionIsCalled
            {
                [SetUp]
                public void ForceRepoToReturnEmotion()
                {
                    _fetchedEmotion = _fixture.Create<Emotion>();
                }

                [Test]
                public async Task ThenAlreadyExistsResultIsReturned()
                {
                    var result = await _service.AddEmotion(_emotionName, _emotionDescription);

                    Assert.That(result, Is.EqualTo(EmotionAdditionResult.AlreadyExists));
                }

                [Test]
                public async Task ThenGetByNameIsCalledWithEmotionName()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.Received(1).GetByName(_emotionName);
                }

                [Test]
                public async Task ThenEmotionIsNotAdded()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.DidNotReceive().Add(Arg.Any<Emotion>());
                }

                [Test]
                public async Task ThenSaveChangesIsNotCalled()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.DidNotReceive().SaveChanges();
                }

                [Test]
                public async Task ThenDefinerIsNotCalled()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeDefiner.DidNotReceive().DefineWord(Arg.Any<string>());
                }
            }

            [TestFixture]
            private class AndEmotionDoesNotExistAndDescriptionIsProvided : WhenAddEmotionIsCalled
            {
                [SetUp]
                public void EnsureDescriptionIsNotNull()
                {
                    _emotionDescription = _fixture.Create<string>();
                }

                [Test]
                public async Task ThenResultContainsAddedEmotionWithProvidedDescription()
                {
                    var result = await _service.AddEmotion(_emotionName, _emotionDescription);

                    using (Assert.EnterMultipleScope())
                    {
                        Assert.That(result.Added, Is.True);
                        Assert.That(result.Emotion, Is.Not.Null);
                        Assert.That(result.Emotion!.Name, Is.EqualTo(_emotionName));
                        Assert.That(result.Emotion.Description, Is.EqualTo(_emotionDescription));
                    }
                }

                [Test]
                public async Task ThenDefinerIsNotCalled()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeDefiner.DidNotReceive().DefineWord(Arg.Any<string>());
                }

                [Test]
                public async Task ThenEmotionIsAddedToRepository()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.Received(1)
                        .Add(Arg.Is<Emotion>(e => e.Name == _emotionName && e.Description == _emotionDescription));
                }

                [Test]
                public async Task ThenSaveChangesIsCalled()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.Received(1).SaveChanges();
                }
            }

            [TestFixture]
            private class AndEmotionDoesNotExistAndDescriptionIsNotProvided : WhenAddEmotionIsCalled
            {
                [Test]
                public async Task ThenDefinerIsCalledWithEmotionName()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeDefiner.Received(1).DefineWord(_emotionName);
                }

                [Test]
                public async Task ThenResultContainsEmotionWithDefinitionDescription()
                {
                    var expectedDefintion = _fetchedDefinition.Meanings.First().Definition;
                    var result = await _service.AddEmotion(_emotionName, _emotionDescription);

                    using (Assert.EnterMultipleScope())
                    {
                        Assert.That(result.Added, Is.True);
                        Assert.That(result.Emotion, Is.Not.Null);
                        Assert.That(result.Emotion!.Name, Is.EqualTo(_emotionName));
                        Assert.That(result.Emotion.Description, Is.EqualTo(expectedDefintion));
                    }
                }

                [Test]
                public async Task ThenEmotionIsAddedWithDefinitionDescription()
                {
                    var expectedDefintion = _fetchedDefinition.Meanings.First().Definition;

                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);
                    
                    _fakeEmotionRepo.Received(1)
                        .Add(Arg.Is<Emotion>(e => e.Name == _emotionName && e.Description == expectedDefintion));
                }

                [Test]
                public async Task ThenSaveChangesIsCalled()
                {
                    _ = await _service.AddEmotion(_emotionName, _emotionDescription);

                    _fakeEmotionRepo.Received(1).SaveChanges();
                }
            }
        }
    }
}
