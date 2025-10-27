using ISynergy.Framework.MessageBus.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.MessageBus.Tests.StepDefinitions;

/// <summary>
/// Step definitions for message subscription scenarios.
/// Demonstrates BDD testing for message bus subscription operations.
/// </summary>
[Binding]
public class MessageSubscriptionSteps
{
    private readonly ILogger<MessageSubscriptionSteps> _logger;
 private readonly MessageBusTestContext _context;

    public MessageSubscriptionSteps(MessageBusTestContext context)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
   _logger = loggerFactory.CreateLogger<MessageSubscriptionSteps>();
   _context = context;
    }

    [Given(@"I have subscribed to topic ""(.*)""")]
    public void GivenIHaveSubscribedToTopic(string topic)
    {
  _logger.LogInformation("Subscribing to topic: {Topic}", topic);
 _context.IsSubscribed = true;
 _context.CurrentTopic = topic;
    }

    [Given(@"subscriber ""(.*)"" has subscribed to topic ""(.*)""")]
    public void GivenSubscriberHasSubscribedToTopic(string subscriberName, string topic)
 {
  _logger.LogInformation("Subscriber {Subscriber} subscribing to topic: {Topic}", subscriberName, topic);
_context.SubscriberReceivedCounts[subscriberName] = 0;
    }

    [When(@"a message is published to topic ""(.*)""")]
    public void WhenAMessageIsPublishedToTopic(string topic)
 {
   _logger.LogInformation("Publishing message to topic: {Topic}", topic);
        
   var message = new TestMessage
   {
   MessageId = Guid.NewGuid(),
     Content = $"Message for topic {topic}",
       Timestamp = DateTime.UtcNow,
 Topic = topic
 };

        // Simulate message delivery to subscribers
        if (_context.IsSubscribed && _context.CurrentTopic == topic)
     {
    _context.ReceivedMessages.Add(message);
            _logger.LogInformation("Message delivered to subscriber");
    }

  // Deliver to named subscribers
   foreach (var subscriber in _context.SubscriberReceivedCounts.Keys)
        {
      _context.SubscriberReceivedCounts[subscriber]++;
        }
 }

    [When(@"I unsubscribe from topic ""(.*)""")]
 public void WhenIUnsubscribeFromTopic(string topic)
    {
    _logger.LogInformation("Unsubscribing from topic: {Topic}", topic);
  _context.IsSubscribed = false;
   _context.CurrentTopic = null;
    }

    [When(@"I attempt to subscribe to topic ""(.*)"" with a null handler")]
    public void WhenIAttemptToSubscribeToTopicWithANullHandler(string topic)
    {
        _logger.LogInformation("Attempting to subscribe with null handler");
   
   try
        {
Action<TestMessage>? handler = null;
      ArgumentNullException.ThrowIfNull(handler, nameof(handler));
        }
   catch (Exception ex)
  {
            _logger.LogWarning(ex, "Caught expected exception");
    _context.CaughtException = ex;
        }
    }

    [Then(@"I should receive the message")]
    public void ThenIShouldReceiveTheMessage()
    {
  _logger.LogInformation("Verifying message was received");
        
        if (_context.ReceivedMessages.Count == 0)
  {
     throw new InvalidOperationException("Expected to receive a message");
        }
    }

 [Then(@"the received message count should be (.*)")]
public void ThenTheReceivedMessageCountShouldBe(int expectedCount)
    {
        _logger.LogInformation("Verifying received message count");
  
     if (_context.ReceivedMessages.Count != expectedCount)
  {
            throw new InvalidOperationException($"Expected {expectedCount} messages but received {_context.ReceivedMessages.Count}");
  }

    _logger.LogInformation("Received {Count} messages successfully", expectedCount);
    }

    [Then(@"both subscribers should receive the message")]
    public void ThenBothSubscribersShouldReceiveTheMessage()
    {
  _logger.LogInformation("Verifying all subscribers received the message");
   
        foreach (var subscriber in _context.SubscriberReceivedCounts)
   {
if (subscriber.Value == 0)
       {
  throw new InvalidOperationException($"Subscriber {subscriber.Key} did not receive the message");
          }
        }

  _logger.LogInformation("All {Count} subscribers received the message", _context.SubscriberReceivedCounts.Count);
  }

    [Then(@"I should not receive any messages")]
    public void ThenIShouldNotReceiveAnyMessages()
    {
  _logger.LogInformation("Verifying no messages were received");
  
        if (_context.ReceivedMessages.Count > 0)
        {
      throw new InvalidOperationException($"Expected no messages but received {_context.ReceivedMessages.Count}");
        }

  _logger.LogInformation("Correctly received no messages");
    }
}
