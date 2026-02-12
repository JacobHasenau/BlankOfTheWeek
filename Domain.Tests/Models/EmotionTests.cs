using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Domain.Models;
using NSubstitute;
using NUnit.Framework;

namespace Domain.Tests.Models;

[TestFixture]
public abstract class GivenAnEmotion
{
    private IFixture _fixture;
    private IEmotionState _state;
    private Emotion _emotion;

    [OneTimeSetUp]
    public void CreateFixture()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
    }

    [SetUp]
    public void CreateEmotion()
    {
        _state = _fixture.Create<IEmotionState>();
        _emotion = new Emotion(_state);
    }

    [TestFixture]
    private class WhenCallingNewDescriptionFound : GivenAnEmotion
    {
        private DefinedWord _newDefinition;

        [SetUp]
        public void CreateDefinedWord()
        {
            _newDefinition = _fixture.Create<DefinedWord>();
        }

        [TestFixture]
        private class AndDefinitionHasMeanings : WhenCallingNewDescriptionFound
        {
            [Test]
            public void ThenStatesDescriptionFoundMethodCalled()
            {
                _emotion.NewDescriptionFound(_newDefinition);
                _state.Received(1).DescriptionFound(_newDefinition.Meanings.First().Definition, 0);
            }
        }

        [TestFixture]
        private class AndDefinitionHasNoMeanings : WhenCallingNewDescriptionFound
        {
            [SetUp]
            public void EmptyDefinedWordsMeanings()
            {
                _newDefinition = _newDefinition with { Meanings = [] };
            }

            [Test]
            public void ThenStatesDescriptionFoundMethodNotCalled()
            {
                _emotion.NewDescriptionFound(_newDefinition);
                _state.DidNotReceiveWithAnyArgs().DescriptionFound(Arg.Any<string>(), Arg.Any<float>());
            }
        }
    }

    [TestFixture]
    private class WhenCallingEmotionFetched : GivenAnEmotion
    {
        [Test]
        public void ThenStateReturnedIsCalled()
        {
            var emotionBeforeCallTime = DateTime.UtcNow;
            _emotion.EmotionFetched();
            var emotionAfterCallTime = DateTime.UtcNow;
            _state.Received(1).StateReturned(Arg.Is<DateTime>(dt => emotionBeforeCallTime <= dt && emotionAfterCallTime >= dt));
        }
    }
}