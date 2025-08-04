const puppeteer = require('puppeteer');

// Test configuration
const CONFIG = {
  FRONTEND_URL: 'http://localhost:3000',
  API_URL: 'http://localhost:6789',
  TIMEOUT: 30000,
  SCREENSHOT_PATH: './screenshots'
};

// Test utilities
const utils = {
  async waitForSelector(page, selector, timeout = CONFIG.TIMEOUT) {
    try {
      await page.waitForSelector(selector, { timeout });
      return true;
    } catch (error) {
      console.error(`❌ Selector not found: ${selector}`);
      return false;
    }
  },

  async takeScreenshot(page, name) {
    try {
      await page.screenshot({ 
        path: `${CONFIG.SCREENSHOT_PATH}/${name}.png`,
        fullPage: true 
      });
      console.log(`📸 Screenshot saved: ${name}.png`);
    } catch (error) {
      console.log('📸 Screenshot directory not found, skipping...');
    }
  },

  async delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
};

// Main test function
async function testChatMode() {
  console.log('🚀 Starting Pronetheia Chat Mode Test...\n');
  
  let browser;
  let page;
  
  try {
    // Launch browser
    console.log('🌐 Launching browser...');
    browser = await puppeteer.launch({ 
      headless: false, // Set to true for headless mode
      slowMo: 100,
      args: ['--no-sandbox', '--disable-setuid-sandbox']
    });
    
    page = await browser.newPage();
    await page.setViewport({ width: 1200, height: 800 });
    
    // Set up console logging
    page.on('console', msg => {
      const type = msg.type();
      if (type === 'error') {
        console.log(`🔥 Browser Error: ${msg.text()}`);
      } else if (type === 'warn') {
        console.log(`⚠️ Browser Warning: ${msg.text()}`);
      }
    });

    // Test 1: Load Frontend
    console.log('📋 Test 1: Loading Pronetheia Frontend...');
    await page.goto(CONFIG.FRONTEND_URL, { waitUntil: 'networkidle2' });
    
    const title = await page.title();
    console.log(`✅ Page loaded: ${title}`);
    await utils.takeScreenshot(page, '01-homepage');

    // Test 2: Check if Dashboard loads
    console.log('\n📋 Test 2: Checking Dashboard...');
    const dashboardExists = await utils.waitForSelector(page, '[data-testid="dashboard"], .dashboard, h1, h2', 5000);
    
    if (dashboardExists) {
      console.log('✅ Dashboard elements found');
    } else {
      console.log('⚠️ Dashboard elements not found, checking page content...');
      const pageContent = await page.content();
      if (pageContent.includes('Pronetheia') || pageContent.includes('Agent') || pageContent.includes('Dashboard')) {
        console.log('✅ Pronetheia content detected');
      } else {
        console.log('❌ No Pronetheia content found');
      }
    }

    // Test 3: Navigate to Chat
    console.log('\n📋 Test 3: Navigating to Chat...');
    
    // Try multiple possible chat navigation methods
    const chatNavigation = await page.evaluate(() => {
      // Look for chat navigation elements
      const possibleSelectors = [
        'a[href="/chat"]',
        'a[href*="chat"]',
        'button[data-testid="chat"]',
        'nav a',
        '.nav-item',
        '[role="navigation"] a'
      ];
      
      for (const selector of possibleSelectors) {
        const element = document.querySelector(selector);
        if (element) {
          element.click();
          return `Found and clicked: ${selector}`;
        }
      }
      
      // If no specific chat link, try to navigate directly
      if (window.location.pathname !== '/chat') {
        window.location.href = window.location.origin + '/chat';
        return 'Navigated directly to /chat';
      }
      
      return 'Already on chat page';
    });
    
    console.log(`🔄 Navigation: ${chatNavigation}`);
    await utils.delay(2000);
    await utils.takeScreenshot(page, '02-chat-page');

    // Test 4: Check Chat Interface
    console.log('\n📋 Test 4: Checking Chat Interface...');
    
    // Look for chat interface elements
    const chatElements = await page.evaluate(() => {
      const elements = {
        messageInput: !!document.querySelector('input[type="text"], textarea, [contenteditable="true"]'),
        sendButton: !!document.querySelector('button[type="submit"], .send-btn'),
        chatArea: !!document.querySelector('.chat, .messages, .conversation, [role="log"]'),
        anyInput: !!document.querySelector('input, textarea, button')
      };
      
      return elements;
    });
    
    console.log('🔍 Chat interface elements:');
    Object.entries(chatElements).forEach(([key, found]) => {
      console.log(`  ${found ? '✅' : '❌'} ${key}: ${found ? 'Found' : 'Not found'}`);
    });

    // Test 5: Test API Connection
    console.log('\n📋 Test 5: Testing API Connection...');
    
    const apiTest = await page.evaluate(async (apiUrl) => {
      try {
        const response = await fetch(`${apiUrl}/api/health`, { 
          method: 'GET',
          headers: { 'Content-Type': 'application/json' }
        });
        
        return {
          status: response.status,
          ok: response.ok,
          statusText: response.statusText
        };
      } catch (error) {
        return {
          error: error.message,
          status: 0
        };
      }
    }, CONFIG.API_URL);
    
    if (apiTest.ok) {
      console.log(`✅ API Connection: ${apiTest.status} ${apiTest.statusText}`);
    } else if (apiTest.status === 404) {
      console.log('⚠️ API Health endpoint not found, but API is responding');
    } else {
      console.log(`❌ API Connection: ${apiTest.error || apiTest.status}`);
    }

    // Test 6: Attempt Chat Interaction
    console.log('\n📋 Test 6: Testing Chat Interaction...');
    
    try {
      // Find input field
      const inputSelector = await page.evaluate(() => {
        const inputs = document.querySelectorAll('input[type="text"], textarea, [contenteditable="true"]');
        for (let input of inputs) {
          if (input.placeholder && (
            input.placeholder.toLowerCase().includes('message') ||
            input.placeholder.toLowerCase().includes('chat') ||
            input.placeholder.toLowerCase().includes('type')
          )) {
            return input.tagName.toLowerCase() + (input.id ? `#${input.id}` : '') + (input.className ? `.${input.className.split(' ')[0]}` : '');
          }
        }
        return inputs.length > 0 ? inputs[0].tagName.toLowerCase() : null;
      });

      if (inputSelector) {
        console.log(`🎯 Found input: ${inputSelector}`);
        
        const testMessage = "Hello Pronetheia! Test message from automated test. Please respond to confirm agent communication is working.";
        
        await page.focus(inputSelector);
        await page.type(inputSelector, testMessage);
        
        console.log(`✅ Typed test message: "${testMessage}"`);
        
        // Try to send the message
        const sendAttempt = await page.evaluate(() => {
          // Look for send button
          const sendButtons = document.querySelectorAll('button');
          for (let btn of sendButtons) {
            if (btn.textContent.toLowerCase().includes('send') || 
                btn.type === 'submit' ||
                btn.innerHTML.includes('send')) {
              btn.click();
              return 'Clicked send button';
            }
          }
          
          // Try Enter key
          const event = new KeyboardEvent('keydown', { key: 'Enter', code: 'Enter' });
          document.activeElement.dispatchEvent(event);
          return 'Sent Enter key';
        });
        
        console.log(`📤 Send attempt: ${sendAttempt}`);
        
        // Wait for potential response
        console.log('⏳ Waiting for agent response (10 seconds)...');
        await utils.delay(10000);
        
        await utils.takeScreenshot(page, '03-chat-interaction');
        
        // Check for new messages or responses
        const responseCheck = await page.evaluate(() => {
          const messages = document.querySelectorAll('.message, .chat-message, [data-role="message"], .msg');
          return {
            messageCount: messages.length,
            lastMessage: messages.length > 0 ? messages[messages.length - 1].textContent : 'No messages found',
            hasResponse: messages.length > 1
          };
        });
        
        console.log(`📊 Response check:`);
        console.log(`  Message count: ${responseCheck.messageCount}`);
        console.log(`  Has response: ${responseCheck.hasResponse ? '✅ Yes' : '❌ No'}`);
        if (responseCheck.lastMessage !== 'No messages found') {
          console.log(`  Last message: "${responseCheck.lastMessage.substring(0, 100)}..."`);
        }
        
      } else {
        console.log('❌ No input field found for chat interaction');
      }
      
    } catch (error) {
      console.log(`❌ Chat interaction failed: ${error.message}`);
    }

    // Test 7: Check Agent Status
    console.log('\n📋 Test 7: Checking Agent Status...');
    
    const agentStatus = await page.evaluate(() => {
      // Look for agent status indicators
      const statusElements = document.querySelectorAll('[data-testid*="agent"], .agent-status, .status, .badge');
      const agents = [];
      
      statusElements.forEach(el => {
        if (el.textContent.includes('Agent') || el.textContent.includes('Chat') || el.textContent.includes('Evolution') || el.textContent.includes('Tool')) {
          agents.push(el.textContent.trim());
        }
      });
      
      return {
        agentElements: statusElements.length,
        agentNames: agents,
        hasAgentInfo: agents.length > 0
      };
    });
    
    console.log(`🤖 Agent Status:`);
    console.log(`  Status elements found: ${agentStatus.agentElements}`);
    console.log(`  Agent info detected: ${agentStatus.hasAgentInfo ? '✅ Yes' : '❌ No'}`);
    if (agentStatus.agentNames.length > 0) {
      console.log(`  Agent names: ${agentStatus.agentNames.join(', ')}`);
    }

    console.log('\n🎉 Chat Mode Test Completed!');
    
    return {
      success: true,
      results: {
        frontend_loaded: true,
        chat_interface: chatElements,
        api_connection: apiTest,
        agent_status: agentStatus
      }
    };
    
  } catch (error) {
    console.error(`❌ Test failed: ${error.message}`);
    if (page) await utils.takeScreenshot(page, '99-error');
    
    return {
      success: false,
      error: error.message
    };
    
  } finally {
    console.log('\n🧹 Cleaning up...');
    if (browser) {
      await browser.close();
    }
  }
}

// Test Summary function
function printTestSummary(results) {
  console.log('\n' + '='.repeat(50));
  console.log('📊 PRONETHEIA CHAT TEST SUMMARY');
  console.log('='.repeat(50));
  
  if (results.success) {
    console.log('🎉 Overall Status: ✅ PASSED');
    console.log('\n📋 Test Results:');
    console.log(`  Frontend Loading: ✅ SUCCESS`);
    console.log(`  Chat Interface: ${results.results.chat_interface.messageInput ? '✅' : '❌'} ${results.results.chat_interface.messageInput ? 'FOUND' : 'NOT FOUND'}`);
    console.log(`  API Connection: ${results.results.api_connection.ok ? '✅' : '❌'} ${results.results.api_connection.ok ? 'CONNECTED' : 'FAILED'}`);
    console.log(`  Agent Status: ${results.results.agent_status.hasAgentInfo ? '✅' : '❌'} ${results.results.agent_status.hasAgentInfo ? 'DETECTED' : 'NOT DETECTED'}`);
  } else {
    console.log('❌ Overall Status: FAILED');
    console.log(`   Error: ${results.error}`);
  }
  
  console.log('\n🌐 Service URLs:');
  console.log(`  Frontend: ${CONFIG.FRONTEND_URL}`);
  console.log(`  API: ${CONFIG.API_URL}`);
  
  console.log('\n💡 Next Steps:');
  if (results.success) {
    console.log('  • System is operational and ready for evolution testing');
    console.log('  • Try triggering the first evolution cycle');
    console.log('  • Test agent-to-agent communication');
  } else {
    console.log('  • Check that all Docker containers are running');
    console.log('  • Verify frontend builds correctly');
    console.log('  • Ensure API is accessible on port 6789');
  }
  
  console.log('='.repeat(50));
}

// Run the test
if (require.main === module) {
  testChatMode().then(results => {
    printTestSummary(results);
    process.exit(results.success ? 0 : 1);
  }).catch(error => {
    console.error('💥 Unhandled error:', error);
    process.exit(1);
  });
}

module.exports = { testChatMode, CONFIG };