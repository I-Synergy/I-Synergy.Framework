using ISynergy.Framework.MessageBus.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.MessageBus.Tests.StepDefinitions;

/// <summary>
/// Step definitions for message publishing scenarios.
/// Demonstrates BDD testing for message bus publishing operations.
/// </summary>
[Binding]
public class MessagePublishingSteps
{
    private readonly ILogger<MessagePublishingSteps> _logger;
    private readonly MessageBusTestContext _context;

    public MessagePublishingSteps(MessageBusTestContext context)
    {
  var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<MessagePublishingSteps>();
        _context = context;
  }

    [Given(@"the message bus is configured")]
    public void GivenTheMessageBusIsConfigured()
    {
  _logger.LogInformation("Configuring message bus for testing");
        // In-memory message bus simulation - no actual Azure dependencies needed for BDD
  }

    [Given(@"I have a test message")]
    public void GivenIHaveATestMessage()
    {
        _logger.LogInformation("Creating test message");
 _context.Message = new TestMessage
        {
      MessageId = Guid.NewGuid(),
         Content = "Test message content",
       Timestamp = DateTime.UtcNow
     };
    }

    [Given(@"I have a null message")]
    public void GivenIHaveANullMessage()
    {
   _logger.LogInformation("Setting message to null");
      _context.Message = null;
    }

    [When(@"I publish the message to topic ""(.*)""")]
    public void WhenIPublishTheMessageToTopic(string topic)
    {
        _logger.LogInformation("Publishing message to topic: {Topic}", topic);
      
        try
        {
 ArgumentNullException.ThrowIfNull(_context.Message, nameof(_context.Message));
      ArgumentException.ThrowIfNullOrWhiteSpace(topic, nameof(topic));

   _context.Message.Topic = topic;
            _context.PublishedMessages.Add(_context.Message);

   // Track messages by topic
            if (!_context.TopicMessages.ContainsKey(topic))
         {
         _context.TopicMessages[topic] = new List<TestMessage>();
     }
        _context.TopicMessages[topic].Add(_context.Message);

   _logger.LogInformation("Message published successfully to topic: {Topic}", topic);
        }
        catch (Exception ex)
   {
      _logger.LogWarning(ex, "Failed to publish message");
      _context.CaughtException = ex;
        }
    }

    [When(@"I attempt to publish the null message to topic ""(.*)""")]
    public void WhenIAttemptToPublishTheNullMessageToTopic(string topic)
 {
  _logger.LogInformation("Attempting to publish null message");
   
  try
    {
   ArgumentNullException.ThrowIfNull(_context.Message, nameof(_context.Message));
        }
  catch (Exception ex)
        {
        _logger.LogWarning(ex, "Caught expected exception");
  _context.CaughtException = ex;
   }
    }

    [Then(@"the message should be published successfully")]
    public void ThenTheMessageShouldBePublishedSuccessfully()
    {
     _logger.LogInformation("Verifying message was published");
        
  if (_context.PublishedMessages.Count == 0)
        {
        throw new InvalidOperationException("Expected message to be published");
  }

  if (_context.CaughtException != null)
     {
      throw new InvalidOperationException($"Expected successful publication but got exception: {_context.CaughtException.Message}");
  }
    }

    [Then(@"the published message count should be (.*)")]
    public void ThenThePublishedMessageCountShouldBe(int expectedCount)
{
        _logger.LogInformation("Verifying published message count");
        
   if (_context.PublishedMessages.Count != expectedCount)
        {
    throw new InvalidOperationException($"Expected {expectedCount} messages but got {_context.PublishedMessages.Count}");
        }

        _logger.LogInformation("Published {Count} messages successfully", expectedCount);
    }

    [Then(@"messages should be published to (.*) different topics")]
    public void ThenMessagesShouldBePublishedToDifferentTopics(int expectedTopicCount)
    {
        _logger.LogInformation("Verifying topic count");
        
        if (_context.TopicMessages.Count != expectedTopicCount)
      {
            throw new InvalidOperationException($"Expected {expectedTopicCount} topics but got {_context.TopicMessages.Count}");
        }

        _logger.LogInformation("Messages published to {Count} different topics", expectedTopicCount);
 }

    [Then(@"an ArgumentNullException should be thrown")]
    public void ThenAnArgumentNullExceptionShouldBeThrown()
    {
        _logger.LogInformation("Verifying ArgumentNullException was thrown");
        
        if (_context.CaughtException is not ArgumentNullException)
{
throw new InvalidOperationException($"Expected ArgumentNullException but got {_context.CaughtException?.GetType().Name ?? "null"}");
        }
    }

    [Then(@"an ArgumentException should be thrown")]
    public void ThenAnArgumentExceptionShouldBeThrown()
  {
  _logger.LogInformation("Verifying ArgumentException was thrown");
        
 if (_context.CaughtException is not ArgumentException)
   {
      throw new InvalidOperationException($"Expected ArgumentException but got {_context.CaughtException?.GetType().Name ?? "null"}");
  }
    }
}
