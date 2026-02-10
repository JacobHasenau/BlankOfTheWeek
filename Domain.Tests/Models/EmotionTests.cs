using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Domain.Models;
using NSubstitute;
using NUnit.Framework;

namespace Domain.Tests.Models;

[TestFixture]
public abstract class GivenAnEmotion
{
    private readonly Fixture _fixture;
    private IEmotionState _state;
    private Emotion _emotion;

    [OneTimeSetUp]
    private void CreateFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
    }

    [SetUp]
    private void CreateEmotion()
    {
        _state = _fixture.Create<IEmotionState>();
        _emotion = new Emotion(_state);
    }

    [TestFixture]
    private class AndCallingNewDescriptionFound : GivenAnEmotion
    {
        [Test]
        public void ThenStatesDescriptionFoundMethodCalled()
        {
            var definedWord = _fixture.Create<DefinedWord>();
            _emotion.NewDescriptionFound(_fixture.Create<DefinedWord>());
            _state.Received().DescriptionFound(definedWord.Meanings.First().Definition, 0)
                ;
        }
    }
}