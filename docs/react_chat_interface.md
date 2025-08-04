# Pronetheia React Chat Interface - Detailed Implementation

**Version:** 1.0  
**Date:** August 3, 2025  
**Context:** VS Code Fork - React Chat Interface Deep Dive

---

## 1. Chat Interface Architecture Overview

### 1.1 Component Hierarchy and Data Flow

```
PronetheiaAIChatProvider (Context Provider)
├── ChatContainer (Main Layout)
│   ├── ChatHeader (Title, Settings, Clear)
│   ├── MessageList (Virtual Scrolling)
│   │   ├── UserMessage (Text + Attachments)
│   │   ├── AIMessage (Response + Code Blocks)
│   │   ├── SystemMessage (Status Updates)
│   │   └── AgentMessage (Agent Communications)
│   ├── FileUploadZone (Drag & Drop)
│   ├── KnowledgeBaseQuickAccess (Recent Files)
│   ├── ChatInput (Rich Text + Controls)
│   └── ChatControls (Send, Voice, Attachments)
├── ContextPanel (Sidebar)
│   ├── ConversationHistory
│   ├── ActiveContext (Files, Selection)
│   └── QuickActions (Templates, Shortcuts)
└── SettingsModal (Configuration)
```

### 1.2 State Management Architecture

```typescript
// Chat State Management with Zustand
interface ChatState {
  // Message Management
  messages: ChatMessage[];
  currentConversation: string;
  conversationHistory: ConversationSummary[];
  
  // Input State
  inputText: string;
  attachments: FileAttachment[];
  isComposing: boolean;
  
  // AI Response State
  isWaitingForResponse: boolean;
  streamingResponse: string;
  responseMetadata: ResponseMetadata;
  
  // Context Management
  activeFiles: string[];
  selectedText: string;
  workspaceContext: WorkspaceInfo;
  knowledgeBaseContext: KBContext[];
  
  // UI State
  isExpanded: boolean;
  showAttachmentPanel: boolean;
  showContextPanel: boolean;
  theme: 'light' | 'dark' | 'auto';
  
  // Actions
  sendMessage: (text: string, attachments?: FileAttachment[]) => Promise<void>;
  addAttachment: (file: File) => void;
  removeAttachment: (id: string) => void;
  clearConversation: () => void;
  loadConversation: (id: string) => void;
  updateContext: (context: Partial<ContextInfo>) => void;
}

const useChatStore = create<ChatState>((set, get) => ({
  messages: [],
  currentConversation: generateConversationId(),
  conversationHistory: [],
  inputText: '',
  attachments: [],
  isComposing: false,
  isWaitingForResponse: false,
  streamingResponse: '',
  responseMetadata: {},
  activeFiles: [],
  selectedText: '',
  workspaceContext: {},
  knowledgeBaseContext: [],
  isExpanded: true,
  showAttachmentPanel: false,
  showContextPanel: false,
  theme: 'auto',
  
  sendMessage: async (text: string, attachments?: FileAttachment[]) => {
    const state = get();
    const message: ChatMessage = {
      id: generateMessageId(),
      text,
      sender: 'user',
      timestamp: new Date(),
      attachments: attachments || state.attachments,
      context: {
        activeFiles: state.activeFiles,
        selectedText: state.selectedText,
        workspaceInfo: state.workspaceContext
      }
    };
    
    set({ 
      messages: [...state.messages, message],
      inputText: '',
      attachments: [],
      isWaitingForResponse: true 
    });
    
    try {
      // Send to VS Code extension host
      await vscode.postMessage({
        type: 'sendMessage', 
        payload: message
      });
    } catch (error) {
      set({ 
        isWaitingForResponse: false,
        messages: [...get().messages, {
          id: generateMessageId(),
          text: `Error: ${error.message}`,
          sender: 'system',
          timestamp: new Date(),
          isError: true
        }]
      });
    }
  },
  
  // ... other actions
}));
```

## 2. Core Chat Components Implementation

### 2.1 Chat Container Component

```typescript
// extensions/pronetheia-ai/media/src/components/ChatContainer.tsx
import React, { useEffect, useRef } from 'react';
import { useChatStore } from '../store/chatStore';
import { ChatHeader } from './ChatHeader';
import { MessageList } from './MessageList';
import { ChatInput } from './ChatInput';
import { FileUploadZone } from './FileUploadZone';
import { ContextPanel } from './ContextPanel';

export const ChatContainer: React.FC = () => {
  const containerRef = useRef<HTMLDivElement>(null);
  const {
    messages,
    isWaitingForResponse,
    showContextPanel,
    theme
  } = useChatStore();
  
  // Apply VS Code theme integration
  useEffect(() => {
    const updateTheme = () => {
      const body = document.body;
      const vscodeTheme = body.getAttribute('data-vscode-theme-kind');
      
      if (vscodeTheme === 'vscode-dark') {
        body.classList.add('dark');
      } else {
        body.classList.remove('dark');
      }
      
      // Apply custom properties from VS Code
      const computedStyle = getComputedStyle(body);
      const root = document.documentElement;
      
      // Sync with VS Code color tokens
      root.style.setProperty('--vscode-foreground', computedStyle.getPropertyValue('--vscode-foreground'));
      root.style.setProperty('--vscode-background', computedStyle.getPropertyValue('--vscode-editor-background'));
      root.style.setProperty('--vscode-input-background', computedStyle.getPropertyValue('--vscode-input-background'));
    };
    
    updateTheme();
    
    // Listen for theme changes
    const observer = new MutationObserver(updateTheme);
    observer.observe(document.body, { attributes: true, attributeFilter: ['data-vscode-theme-kind'] });
    
    return () => observer.disconnect();
  }, []);
  
  return (
    <div 
      ref={containerRef}
      className={`chat-container ${showContextPanel ? 'with-context' : ''}`}
      style={{
        height: '100vh',
        display: 'flex',
        flexDirection: 'column',
        background: 'var(--vscode-editor-background)',
        color: 'var(--vscode-foreground)'
      }}
    >
      <ChatHeader />
      
      <div className="chat-body" style={{ 
        flex: 1, 
        display: 'flex',
        overflow: 'hidden'
      }}>
        <div className="main-chat" style={{ 
          flex: 1, 
          display: 'flex', 
          flexDirection: 'column',
          minWidth: 0
        }}>
          <MessageList messages={messages} />
          <FileUploadZone />
          <ChatInput disabled={isWaitingForResponse} />
        </div>
        
        {showContextPanel && (
          <ContextPanel />
        )}
      </div>
    </div>
  );
};
```

### 2.2 Advanced Message List with Virtual Scrolling

```typescript
// extensions/pronetheia-ai/media/src/components/MessageList.tsx
import React, { useEffect, useRef, useMemo } from 'react';
import { FixedSizeList as List } from 'react-window';
import { MessageBubble } from './MessageBubble';
import { LoadingIndicator } from './LoadingIndicator';
import { useChatStore } from '../store/chatStore';

interface MessageListProps {
  messages: ChatMessage[];
}

export const MessageList: React.FC<MessageListProps> = ({ messages }) => {
  const listRef = useRef<List>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const { isWaitingForResponse, streamingResponse } = useChatStore();
  
  // Auto-scroll to bottom on new messages
  useEffect(() => {
    if (listRef.current) {
      listRef.current.scrollToItem(messages.length - 1, 'end');
    }
  }, [messages.length]);
  
  // Calculate dynamic message heights
  const messageSizes = useMemo(() => {
    return messages.map(message => {
      // Base height
      let height = 60;
      
      // Add height for text content (rough estimate)
      height += Math.ceil(message.text.length / 80) * 20;
      
      // Add height for attachments
      if (message.attachments?.length) {
        height += message.attachments.length * 40 + 20;
      }
      
      // Add height for code blocks
      if (message.codeBlocks?.length) {
        height += message.codeBlocks.reduce((acc, block) => 
          acc + Math.ceil(block.code.split('\n').length * 16) + 40, 0);
      }
      
      // Add height for file changes
      if (message.fileChanges?.length) {
        height += message.fileChanges.length * 30 + 20;
      }
      
      return Math.max(height, 80); // Minimum height
    });
  }, [messages]);
  
  const renderMessage = ({ index, style }: { index: number; style: React.CSSProperties }) => {
    const message = messages[index];
    
    return (
      <div style={style}>
        <MessageBubble 
          message={message}
          isLast={index === messages.length - 1}
        />
      </div>
    );
  };
  
  const totalHeight = messageSizes.reduce((acc, height) => acc + height, 0);
  
  return (
    <div 
      ref={containerRef}
      className="message-list"
      style={{ 
        flex: 1,
        overflow: 'hidden',
        padding: '8px',
        background: 'var(--vscode-editor-background)'
      }}
    >
      {messages.length === 0 ? (
        <WelcomeMessage />
      ) : (
        <List
          ref={listRef}
          height={containerRef.current?.clientHeight || 400}
          itemCount={messages.length}
          itemSize={(index) => messageSizes[index]}
          itemData={messages}
        >
          {renderMessage}
        </List>
      )}
      
      {/* Streaming response indicator */}
      {isWaitingForResponse && (
        <div className="streaming-container" style={{
          padding: '12px',
          borderTop: '1px solid var(--vscode-widget-border)'
        }}>
          {streamingResponse ? (
            <MessageBubble 
              message={{
                id: 'streaming',
                text: streamingResponse,
                sender: 'ai',
                timestamp: new Date(),
                isStreaming: true
              }}
              isLast={true}
            />
          ) : (
            <LoadingIndicator text="Pronetheia is thinking..." />
          )}
        </div>
      )}
    </div>
  );
};

const WelcomeMessage: React.FC = () => (
  <div className="welcome-message" style={{
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    height: '100%',
    textAlign: 'center',
    color: 'var(--vscode-descriptionForeground)'
  }}>
    <div style={{ fontSize: '48px', marginBottom: '16px' }}>🤖</div>
    <h2 style={{ 
      color: 'var(--vscode-foreground)',
      marginBottom: '8px'
    }}>
      Welcome to Pronetheia AI
    </h2>
    <p style={{ maxWidth: '400px', lineHeight: '1.5' }}>
      I'm your AI development assistant. I can help you generate projects, 
      write code, manage configurations, and coordinate with specialized agents.
    </p>
    <div style={{ 
      marginTop: '24px',
      display: 'flex',
      gap: '8px',
      flexWrap: 'wrap',
      justifyContent: 'center'
    }}>
      <QuickActionButton text="Generate a new project" />
      <QuickActionButton text="Analyze uploaded documents" />
      <QuickActionButton text="Configure environment settings" />
    </div>
  </div>
);

const QuickActionButton: React.FC<{ text: string }> = ({ text }) => {
  const { sendMessage } = useChatStore();
  
  return (
    <button
      onClick={() => sendMessage(text)}
      style={{
        padding: '8px 12px',
        background: 'var(--vscode-button-background)',
        color: 'var(--vscode-button-foreground)',
        border: 'none',
        borderRadius: '4px',
        cursor: 'pointer',
        fontSize: '12px'
      }}
    >
      {text}
    </button>
  );
};
```

### 2.3 Rich Message Bubble Component

```typescript
// extensions/pronetheia-ai/media/src/components/MessageBubble.tsx
import React, { useState } from 'react';
import { CodeBlock } from './CodeBlock';
import { FileAttachmentList } from './FileAttachmentList';
import { FileChangesList } from './FileChangesList';
import { MessageActions } from './MessageActions';

interface MessageBubbleProps {
  message: ChatMessage;
  isLast: boolean;
}

export const MessageBubble: React.FC<MessageBubbleProps> = ({ message, isLast }) => {
  const [isExpanded, setIsExpanded] = useState(true);
  const isUser = message.sender === 'user';
  const isSystem = message.sender === 'system';
  const isAgent = message.sender?.startsWith('agent:');
  
  const getMessageStyle = () => {
    const baseStyle = {
      margin: '8px 0',
      padding: '12px 16px',
      borderRadius: '8px',
      maxWidth: '85%',
      wordWrap: 'break-word' as const,
      position: 'relative' as const
    };
    
    if (isUser) {
      return {
        ...baseStyle,
        background: 'var(--vscode-inputValidation-infoBorder)',
        color: 'var(--vscode-input-foreground)',
        marginLeft: 'auto',
        borderBottomRightRadius: '4px'
      };
    } else if (isSystem) {
      return {
        ...baseStyle,
        background: 'var(--vscode-inputValidation-warningBackground)',
        color: 'var(--vscode-inputValidation-warningForeground)',
        margin: '8px auto',
        textAlign: 'center' as const,
        maxWidth: '70%',
        fontSize: '14px'
      };
    } else if (isAgent) {
      return {
        ...baseStyle,
        background: 'var(--vscode-badge-background)',
        color: 'var(--vscode-badge-foreground)',
        borderLeft: '3px solid var(--vscode-progressBar-background)',
        borderBottomLeftRadius: '4px'
      };
    } else {
      return {
        ...baseStyle,
        background: 'var(--vscode-editor-inactiveSelectionBackground)',
        color: 'var(--vscode-editor-foreground)',
        borderBottomLeftRadius: '4px'
      };
    }
  };
  
  const formatTimestamp = (timestamp: Date) => {
    return new Intl.DateTimeFormat('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).format(timestamp);
  };
  
  return (
    <div 
      className={`message-bubble ${message.sender}`}
      style={{ 
        display: 'flex', 
        flexDirection: 'column',
        alignItems: isUser ? 'flex-end' : 'flex-start'
      }}
    >
      {/* Message Header */}
      <div style={{
        fontSize: '11px',
        color: 'var(--vscode-descriptionForeground)',
        marginBottom: '4px',
        display: 'flex',
        alignItems: 'center',
        gap: '8px'
      }}>
        <SenderAvatar sender={message.sender} />
        <span>{getSenderDisplayName(message.sender)}</span>
        <span>{formatTimestamp(message.timestamp)}</span>
        {message.isStreaming && (
          <span style={{ color: 'var(--vscode-progressBar-background)' }}>
            ● Streaming...
          </span>
        )}
      </div>
      
      {/* Main Message Content */}
      <div style={getMessageStyle()}>
        {/* Text Content with Markdown Support */}
        <MessageText 
          text={message.text} 
          isStreaming={message.isStreaming}
        />
        
        {/* Attachments */}
        {message.attachments && message.attachments.length > 0 && (
          <FileAttachmentList 
            attachments={message.attachments}
            isExpanded={isExpanded}
            onToggleExpanded={() => setIsExpanded(!isExpanded)}
          />
        )}
        
        {/* Code Blocks */}
        {message.codeBlocks && message.codeBlocks.length > 0 && (
          <div style={{ marginTop: '12px' }}>
            {message.codeBlocks.map((codeBlock, index) => (
              <CodeBlock 
                key={index}
                code={codeBlock.code}
                language={codeBlock.language}
                filename={codeBlock.filename}
                showCopyButton={true}
                showApplyButton={!isUser}
              />
            ))}
          </div>
        )}
        
        {/* File Changes */}
        {message.fileChanges && message.fileChanges.length > 0 && (
          <FileChangesList 
            changes={message.fileChanges}
            onApplyChange={(change) => {
              vscode.postMessage({
                type: 'applyFileChange',
                payload: change
              });
            }}
          />
        )}
        
        {/* Project Structure */}
        {message.projectStructure && (
          <ProjectStructureViewer 
            structure={message.projectStructure}
            onCreateProject={() => {
              vscode.postMessage({
                type: 'createProject',
                payload: message.projectStructure
              });
            }}
          />
        )}
      </div>
      
      {/* Message Actions */}
      {!isSystem && (
        <MessageActions 
          message={message}
          isLast={isLast}
        />
      )}
    </div>
  );
};

const SenderAvatar: React.FC<{ sender: string }> = ({ sender }) => {
  const getAvatarContent = () => {
    if (sender === 'user') return '👤';
    if (sender === 'system') return '⚙️';
    if (sender?.startsWith('agent:')) {
      const agentType = sender.split(':')[1];
      const agentEmojis: Record<string, string> = {
        'backend': '⚙️',
        'frontend': '🎨',
        'database': '🗄️',
        'security': '🔒',
        'testing': '🧪',
        'deployment': '🚀'
      };
      return agentEmojis[agentType] || '🤖';
    }
    return '🤖';
  };
  
  return (
    <span style={{
      width: '16px',
      height: '16px',
      borderRadius: '50%',
      display: 'inline-flex',
      alignItems: 'center',
      justifyContent: 'center',
      fontSize: '10px'
    }}>
      {getAvatarContent()}
    </span>
  );
};

const getSenderDisplayName = (sender: string): string => {
  if (sender === 'user') return 'You';
  if (sender === 'system') return 'System';
  if (sender?.startsWith('agent:')) {
    const agentType = sender.split(':')[1];
    return `${agentType.charAt(0).toUpperCase() + agentType.slice(1)} Agent`;
  }
  return 'Pronetheia AI';
};
```

### 2.4 Advanced Chat Input with Rich Features

```typescript
// extensions/pronetheia-ai/media/src/components/ChatInput.tsx
import React, { useState, useRef, useEffect, useCallback } from 'react';
import { useChatStore } from '../store/chatStore';
import { AttachmentButton } from './AttachmentButton';
import { VoiceInputButton } from './VoiceInputButton';
import { ContextButton } from './ContextButton';

interface ChatInputProps {
  disabled?: boolean;
}

export const ChatInput: React.FC<ChatInputProps> = ({ disabled = false }) => {
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const [localText, setLocalText] = useState('');
  const [suggestions, setSuggestions] = useState<string[]>([]);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const [selectedSuggestionIndex, setSelectedSuggestionIndex] = useState(-1);
  
  const {
    inputText,
    attachments,
    sendMessage,
    isWaitingForResponse,
    updateInputText
  } = useChatStore();
  
  // Sync with store
  useEffect(() => {
    setLocalText(inputText);
  }, [inputText]);
  
  // Auto-resize textarea
  const adjustTextareaHeight = useCallback(() => {
    const textarea = textareaRef.current;
    if (textarea) {
      textarea.style.height = 'auto';
      const scrollHeight = textarea.scrollHeight;
      const maxHeight = 200; // Maximum height in pixels
      textarea.style.height = `${Math.min(scrollHeight, maxHeight)}px`;
    }
  }, []);
  
  useEffect(() => {
    adjustTextareaHeight();
  }, [localText, adjustTextareaHeight]);
  
  // Handle input changes with smart suggestions
  const handleInputChange = useCallback(async (value: string) => {
    setLocalText(value);
    updateInputText(value);
    
    // Trigger smart suggestions
    if (value.length > 2) {
      const smartSuggestions = await getSmartSuggestions(value);
      setSuggestions(smartSuggestions);
      setShowSuggestions(smartSuggestions.length > 0);
      setSelectedSuggestionIndex(-1);
    } else {
      setShowSuggestions(false);
    }
  }, [updateInputText]);
  
  // Handle keyboard shortcuts
  const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
    // Send message on Enter (without Shift)
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      if (localText.trim() && !disabled) {
        handleSendMessage();
      }
    }
    
    // Handle suggestion navigation
    if (showSuggestions) {
      if (e.key === 'ArrowDown') {
        e.preventDefault();
        setSelectedSuggestionIndex(prev => 
          Math.min(prev + 1, suggestions.length - 1)
        );
      } else if (e.key === 'ArrowUp') {
        e.preventDefault();
        setSelectedSuggestionIndex(prev => Math.max(prev - 1, -1));
      } else if (e.key === 'Tab' && selectedSuggestionIndex >= 0) {
        e.preventDefault();
        const selectedSuggestion = suggestions[selectedSuggestionIndex];
        setLocalText(selectedSuggestion);
        updateInputText(selectedSuggestion);
        setShowSuggestions(false);
      } else if (e.key === 'Escape') {
        setShowSuggestions(false);
      }
    }
    
    // Global shortcuts
    if (e.ctrlKey || e.metaKey) {
      switch (e.key) {
        case 'k':
          e.preventDefault();
          textareaRef.current?.focus();
          break;
        case 'u':
          e.preventDefault();
          // Trigger file upload
          document.getElementById('file-upload')?.click();
          break;
      }
    }
  }, [localText, disabled, showSuggestions, selectedSuggestionIndex, suggestions]);
  
  const handleSendMessage = useCallback(async () => {
    if (!localText.trim() || disabled) return;
    
    await sendMessage(localText.trim(), attachments);
    setLocalText('');
    setShowSuggestions(false);
    
    // Focus back to input
    setTimeout(() => {
      textareaRef.current?.focus();
    }, 100);
  }, [localText, disabled, sendMessage, attachments]);
  
  // Smart suggestions based on context
  const getSmartSuggestions = async (input: string): Promise<string[]> => {
    // This would integrate with your AI backend for contextual suggestions
    const commonSuggestions = [
      'Create a new React component for',
      'Generate a REST API endpoint for',
      'Write unit tests for',
      'Add error handling to',
      'Optimize the performance of',
      'Create a database migration for',
      'Set up CI/CD pipeline for',
      'Add authentication to',
      'Create configuration for',
      'Generate documentation for'
    ];
    
    // Filter based on input
    return commonSuggestions
      .filter(suggestion => 
        suggestion.toLowerCase().includes(input.toLowerCase())
      )
      .slice(0, 5);
  };
  
  return (
    <div className="chat-input-container" style={{
      padding: '12px 16px',
      borderTop: '1px solid var(--vscode-widget-border)',
      background: 'var(--vscode-editor-background)'
    }}>
      {/* Attachment Preview */}
      {attachments.length > 0 && (
        <AttachmentPreview attachments={attachments} />
      )}
      
      {/* Smart Suggestions */}
      {showSuggestions && (
        <SuggestionsList 
          suggestions={suggestions}
          selectedIndex={selectedSuggestionIndex}
          onSelectSuggestion={(suggestion) => {
            setLocalText(suggestion);
            setShowSuggestions(false);
            textareaRef.current?.focus();
          }}
        />
      )}
      
      {/* Input Area */}
      <div style={{
        display: 'flex',
        alignItems: 'flex-end',
        gap: '8px',
        position: 'relative'
      }}>
        {/* Context Button */}
        <ContextButton />
        
        {/* Text Input */}
        <div style={{ flex: 1, position: 'relative' }}>
          <textarea
            ref={textareaRef}
            value={localText}
            onChange={(e) => handleInputChange(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder={disabled ? 'AI is responding...' : 'Describe your project or ask a question...'}
            disabled={disabled}
            style={{
              width: '100%',
              minHeight: '36px',
              maxHeight: '200px',
              padding: '8px 12px',
              border: '1px solid var(--vscode-input-border)',
              borderRadius: '6px',
              background: 'var(--vscode-input-background)',
              color: 'var(--vscode-input-foreground)',
              fontSize: '14px',
              lineHeight: '1.4',
              resize: 'none',
              outline: 'none',
              fontFamily: 'inherit'
            }}
          />
          
          {/* Character Count */}
          <div style={{
            position: 'absolute',
            bottom: '4px',
            right: '8px',
            fontSize: '10px',
            color: 'var(--vscode-descriptionForeground)',
            pointerEvents: 'none'
          }}>
            {localText.length}/4000
          </div>
        </div>
        
        {/* Action Buttons */}
        <div style={{ display: 'flex', gap: '4px' }}>
          <AttachmentButton />
          <VoiceInputButton />
          
          <button
            onClick={handleSendMessage}
            disabled={!localText.trim() || disabled}
            style={{
              padding: '8px 12px',
              background: localText.trim() && !disabled 
                ? 'var(--vscode-button-background)' 
                : 'var(--vscode-button-secondaryBackground)',
              color: localText.trim() && !disabled
                ? 'var(--vscode-button-foreground)'
                : 'var(--vscode-button-secondaryForeground)',
              border: 'none',
              borderRadius: '4px',
              cursor: localText.trim() && !disabled ? 'pointer' : 'not-allowed',
              fontSize: '12px',
              fontWeight: '500'
            }}
          >
            {isWaitingForResponse ? '⏳' : '→'}
          </button>
        </div>
      </div>
      
      {/* Keyboard Shortcuts Hint */}
      <div style={{
        fontSize: '10px',
        color: 'var(--vscode-descriptionForeground)',
        marginTop: '4px',
        textAlign: 'center'
      }}>
        Press Enter to send, Shift+Enter for new line, Ctrl+K to focus, Ctrl+U to upload
      </div>
    </div>
  );
};

// Additional components would be implemented similarly...
const AttachmentPreview: React.FC<{ attachments: FileAttachment[] }> = ({ attachments }) => {
  // Implementation for showing attachment previews
  return null;
};

const SuggestionsList: React.FC<{
  suggestions: string[];
  selectedIndex: number;
  onSelectSuggestion: (suggestion: string) => void;
}> = ({ suggestions, selectedIndex, onSelectSuggestion }) => {
  // Implementation for smart suggestions dropdown
  return null;
};
```

## 3. VS Code Integration Layer

### 3.1 WebView Communication Bridge

```typescript
// extensions/pronetheia-ai/src/webviewMessageHandler.ts
export class WebviewMessageHandler {
  constructor(
    private webview: vscode.Webview,
    private pronetheiaService: PronetheiaService,
    private context: vscode.ExtensionContext
  ) {
    this.setupMessageHandling();
  }
  
  private setupMessageHandling(): void {
    this.webview.onDidReceiveMessage(async (message) => {
      try {
        switch (message.type) {
          case 'sendMessage':
            await this.handleSendMessage(message.payload);
            break;
            
          case 'uploadFiles':
            await this.handleFileUpload(message.payload);
            break;
            
          case 'applyFileChange':
            await this.handleApplyFileChange(message.payload);
            break;
            
          case 'createProject':
            await this.handleCreateProject(message.payload);
            break;
            
          case 'getContext':
            await this.handleGetContext();
            break;
            
          default:
            console.warn('Unknown message type:', message.type);
        }
      } catch (error) {
        this.sendErrorToWebview(error);
      }
    });
  }
  
  private async handleSendMessage(payload: ChatMessage): Promise<void> {
    // Add VS Code context to the message
    const enrichedMessage = {
      ...payload,
      context: {
        ...payload.context,
        workspaceRoot: vscode.workspace.workspaceFolders?.[0]?.uri.fsPath,
        activeEditor: vscode.window.activeTextEditor?.document.uri.fsPath,
        selectedText: vscode.window.activeTextEditor?.document.getText(
          vscode.window.activeTextEditor.selection
        ),
        openFiles: vscode.workspace.textDocuments.map(doc => ({
          path: doc.uri.fsPath,
          language: doc.languageId,
          isDirty: doc.isDirty
        }))
      }
    };
    
    // Send to Pronetheia backend
    const response = await this.pronetheiaService.sendMessage(enrichedMessage);
    
    // Handle streaming response
    if (response.isStreaming) {
      this.handleStreamingResponse(response);
    } else {
      this.sendToWebview('aiResponse', response);
    }
  }
  
  private async handleStreamingResponse(response: StreamingAIResponse): Promise<void> {
    for await (const chunk of response.stream) {
      this.sendToWebview('streamingChunk', {
        content: chunk.content,
        isComplete: chunk.isComplete
      });
      
      if (chunk.isComplete) {
        this.sendToWebview('streamingComplete', {
          finalResponse: chunk.finalResponse
        });
      }
    }
  }
  
  private async handleFileUpload(files: FileUploadPayload[]): Promise<void> {
    const uploadResults = await Promise.all(
      files.map(async (file) => {
        try {
          // Process file through knowledge base
          const processed = await this.pronetheiaService.addToKnowledgeBase({
            name: file.name,
            content: file.content,
            type: file.type,
            size: file.size
          });
          
          return { success: true, file: processed };
        } catch (error) {
          return { success: false, error: error.message, fileName: file.name };
        }
      })
    );
    
    this.sendToWebview('fileUploadResults', uploadResults);
  }
  
  private async handleApplyFileChange(change: FileChange): Promise<void> {
    try {
      const uri = vscode.Uri.file(change.filePath);
      
      if (change.type === 'create') {
        // Create new file
        await vscode.workspace.fs.writeFile(uri, Buffer.from(change.content, 'utf8'));
      } else if (change.type === 'modify') {
        // Modify existing file
        const document = await vscode.workspace.openTextDocument(uri);
        const edit = new vscode.WorkspaceEdit();
        
        if (change.range) {
          // Apply specific range change
          const range = new vscode.Range(
            change.range.start.line,
            change.range.start.character,
            change.range.end.line,
            change.range.end.character
          );
          edit.replace(uri, range, change.content);
        } else {
          // Replace entire file
          const fullRange = new vscode.Range(
            0, 0,
            document.lineCount - 1,
            document.lineAt(document.lineCount - 1).text.length
          );
          edit.replace(uri, fullRange, change.content);
        }
        
        await vscode.workspace.applyEdit(edit);
      }
      
      // Open the file if requested
      if (change.openAfterApply) {
        await vscode.window.showTextDocument(uri);
      }
      
      this.sendToWebview