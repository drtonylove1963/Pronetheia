# Pronetheia Knowledge Base File Processing Architecture - Complete Implementation

**Version:** 1.0  
**Date:** August 3, 2025  
**Context:** VS Code Fork - Knowledge Base Deep Dive (Fresh Start)  
**Previous:** Multi-Agent Visualization Dashboard (Complete)

---

## 📋 **Technical Deep Dive Progress Tracker**

- [x] **Item 1: React Chat Interface Implementation** ✅ *Complete*
- [x] **Item 2: Multi-Agent Visualization Dashboard** ✅ *Complete*
- [x] **Item 3: Knowledge Base File Processing Architecture** ← *Currently exploring (Fresh Start)*
- [ ] **Item 4: Project Generation Engine Design**

---

## 1. Knowledge Base Strategic Overview

### 1.1 Vision: The Most Advanced File Intelligence System

**Core Mission:** Transform Pronetheia into the **ChatGPT of development environments** - where every uploaded file becomes intelligent, queryable context that enhances AI responses across all project work.

**Competitive Edge:** While ChatGPT has basic file upload and other tools have limited context, Pronetheia creates **persistent, cross-project intelligence** that gets smarter with every document, image, and spreadsheet uploaded.

### 1.2 Multi-Modal File Processing Matrix

| File Type | Processing Capabilities | AI Context Generated | Business Value |
|-----------|------------------------|---------------------|----------------|
| **📄 PDFs** | Text extraction, table parsing, form detection, TOC analysis | Requirements extraction, process documentation | Legal docs, specs, manuals |
| **🖼️ Images** | OCR, object detection, UI mockup analysis, diagram parsing | Visual descriptions, wireframe analysis | Mockups, screenshots, diagrams |
| **📊 Excel/CSV** | Schema detection, data profiling, relationship mapping | Data insights, business rules | Analytics, configurations |
| **💻 Code Files** | AST parsing, dependency analysis, pattern detection | Architecture insights, API documentation | Existing codebases, examples |
| **📝 Word Docs** | Structure parsing, comment extraction, track changes | Content summaries, requirements lists | Business requirements, specs |
| **🎨 Design Files** | Layer analysis, color extraction, font detection | Design system insights, brand guidelines | Design assets, style guides |

### 1.3 Knowledge Base Architecture

```
┌─ File Upload Interface ─────────────────────────────────────┐
│ Drag & Drop Zone + File Browser + Bulk Upload              │
└─────────────────┬───────────────────────────────────────────┘
                  │
┌─ Intelligent Processing Pipeline ───────────────────────────┐
│ ┌─Multi-Modal┐ ┌─AI Analysis┐ ┌─Relationship┐ ┌─Context─┐  │
│ │Processors  │→│& Insights  │→│Detection    │→│Generation│  │
│ │• PDF       │ │• Summary   │ │• Cross-ref  │ │• Embed   │  │
│ │• Image     │ │• Entities  │ │• Similarity │ │• Search  │  │
│ │• Excel     │ │• Topics    │ │• References │ │• Index   │  │
│ │• Code      │ │• Sentiment │ │• Conflicts  │ │• Graph   │  │
│ └────────────┘ └────────────┘ └─────────────┘ └──────────┘  │
└─────────────────┬───────────────────────────────────────────┘
                  │
┌─ Multi-Database Storage Layer ──────────────────────────────┐
│ ┌─PostgreSQL─┐ ┌─Elasticsearch─┐ ┌─Vector DB─┐ ┌─Graph DB─┐│
│ │Metadata &  │ │Full-text      │ │Semantic   │ │Relations│ │
│ │Structure   │ │Search Index   │ │Embeddings │ │& Context│ │
│ └────────────┘ └───────────────┘ └───────────┘ └─────────┘ │
└─────────────────┬───────────────────────────────────────────┘
                  │
┌─ AI Context Engine ─────────────────────────────────────────┐
│ Real-time context injection for all AI interactions        │
│ • Chat responses enhanced with relevant documents           │
│ • Code generation informed by uploaded specifications       │
│ • Project creation guided by requirements and examples      │
└─────────────────────────────────────────────────────────────┘
```

## 2. Universal File Processing Engine

### 2.1 Core Processing Interface

```typescript
// src/knowledgeBase/core/fileProcessor.ts
export interface ProcessedFile {
  // Core Identification
  id: string;
  originalName: string;
  mimeType: string;
  size: number;
  uploadedAt: Date;
  projectId?: string;
  userId: string;
  
  // Extracted Content
  textContent: string;
  structuredData?: any;
  rawContent?: ArrayBuffer;
  
  // AI-Generated Intelligence
  summary: string;
  keyTopics: string[];
  namedEntities: NamedEntity[];
  sentimentAnalysis?: SentimentResult;
  
  // Processing Metadata
  processingStatus: 'pending' | 'processing' | 'completed' | 'failed' | 'partial';
  processingTime: number;
  processingErrors?: string[];
  confidence: number;
  
  // Content Structure
  sections: ContentSection[];
  extractedElements: ExtractedElement[];
  
  // Search & Discovery
  embeddings: number[];
  searchableText: string;
  tags: string[];
  categories: string[];
  
  // Relationships
  relationships: FileRelationship[];
  crossReferences: CrossReference[];
  
  // Usage Analytics
  accessCount: number;
  lastAccessed?: Date;
  aiContextUsage: number;
  
  // File-Specific Data
  specificData?: PDFData | ImageData | ExcelData | CodeData | DocumentData;
}

export interface ContentSection {
  id: string;
  type: 'heading' | 'paragraph' | 'table' | 'code' | 'image' | 'list' | 'quote';
  content: string;
  level?: number;
  position: Position;
  metadata: Record<string, any>;
  confidence: number;
}

export interface NamedEntity {
  text: string;
  type: 'person' | 'organization' | 'location' | 'technology' | 'concept' | 'requirement' | 'feature';
  confidence: number;
  context: string;
  mentions: EntityMention[];
}

export interface EntityMention {
  sectionId: string;
  position: Position;
  context: string;
}

export interface FileRelationship {
  relatedFileId: string;
  relationshipType: 'references' | 'implements' | 'extends' | 'contradicts' | 'similar' | 'part_of';
  confidence: number;
  description: string;
  evidenceText?: string;
}

export interface CrossReference {
  sourceLocation: Position;
  targetFileId: string;
  targetLocation?: Position;
  referenceType: 'explicit' | 'implicit' | 'semantic';
  confidence: number;
}

export class UniversalFileProcessor {
  private processors = new Map<string, FileProcessorInterface>();
  private aiService: AIAnalysisService;
  private embeddingService: EmbeddingService;
  private relationshipDetector: RelationshipDetectionService;
  
  constructor(
    aiService: AIAnalysisService,
    embeddingService: EmbeddingService,
    relationshipDetector: RelationshipDetectionService
  ) {
    this.aiService = aiService;
    this.embeddingService = embeddingService;
    this.relationshipDetector = relationshipDetector;
    this.initializeProcessors();
  }
  
  private initializeProcessors(): void {
    // Register all specialized processors
    this.processors.set('application/pdf', new AdvancedPDFProcessor());
    this.processors.set('image/png', new IntelligentImageProcessor());
    this.processors.set('image/jpeg', new IntelligentImageProcessor());
    this.processors.set('image/gif', new IntelligentImageProcessor());
    this.processors.set('image/svg+xml', new SVGDiagramProcessor());
    this.processors.set('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', new SmartExcelProcessor());
    this.processors.set('text/csv', new CSVDataProcessor());
    this.processors.set('application/vnd.openxmlformats-officedocument.wordprocessingml.document', new WordDocumentProcessor());
    this.processors.set('text/markdown', new MarkdownProcessor());
    this.processors.set('application/json', new JSONSchemaProcessor());
    this.processors.set('text/javascript', new CodeProcessor());
    this.processors.set('text/typescript', new CodeProcessor());
    this.processors.set('text/python', new CodeProcessor());
    this.processors.set('text/java', new CodeProcessor());
    this.processors.set('text/csharp', new CodeProcessor());
  }
  
  async processFile(
    file: File, 
    userId: string, 
    projectId?: string,
    existingFiles?: ProcessedFile[]
  ): Promise<ProcessedFile> {
    const startTime = Date.now();
    const fileId = `kb_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    
    let processedFile: Partial<ProcessedFile> = {
      id: fileId,
      originalName: file.name,
      mimeType: file.type,
      size: file.size,
      uploadedAt: new Date(),
      projectId,
      userId,
      processingStatus: 'processing',
      accessCount: 0,
      aiContextUsage: 0,
      tags: [],
      categories: [],
      relationships: [],
      crossReferences: [],
      sections: [],
      extractedElements: [],
      namedEntities: []
    };
    
    try {
      // Step 1: Get appropriate processor
      const processor = this.processors.get(file.type);
      if (!processor) {
        throw new Error(`Unsupported file type: ${file.type}`);
      }
      
      // Step 2: Extract raw content and structure
      const extractedContent = await processor.extractContent(file);
      processedFile.textContent = extractedContent.textContent;
      processedFile.structuredData = extractedContent.structuredData;
      processedFile.sections = extractedContent.sections;
      processedFile.extractedElements = extractedContent.elements;
      processedFile.specificData = extractedContent.specificData;
      
      // Step 3: AI-powered content analysis
      const aiAnalysis = await this.aiService.analyzeContent({
        text: extractedContent.textContent,
        fileType: file.type,
        fileName: file.name,
        structuredData: extractedContent.structuredData
      });
      
      processedFile.summary = aiAnalysis.summary;
      processedFile.keyTopics = aiAnalysis.topics;
      processedFile.namedEntities = aiAnalysis.entities;
      processedFile.sentimentAnalysis = aiAnalysis.sentiment;
      processedFile.tags = aiAnalysis.suggestedTags;
      processedFile.categories = aiAnalysis.categories;
      
      // Step 4: Generate embeddings for semantic search
      const embeddingText = [
        processedFile.textContent,
        processedFile.summary,
        processedFile.keyTopics.join(' '),
        processedFile.namedEntities.map(e => e.text).join(' ')
      ].filter(Boolean).join('\n');
      
      processedFile.embeddings = await this.embeddingService.generateEmbeddings(embeddingText);
      processedFile.searchableText = this.createSearchableText(processedFile as ProcessedFile);
      
      // Step 5: Detect relationships with existing files
      if (existingFiles && existingFiles.length > 0) {
        processedFile.relationships = await this.relationshipDetector.findRelationships(
          processedFile as ProcessedFile,
          existingFiles
        );
        
        processedFile.crossReferences = await this.relationshipDetector.findCrossReferences(
          processedFile as ProcessedFile,
          existingFiles
        );
      }
      
      // Step 6: Calculate processing confidence
      processedFile.confidence = this.calculateProcessingConfidence(
        extractedContent,
        aiAnalysis,
        processedFile.embeddings?.length || 0
      );
      
      processedFile.processingStatus = 'completed';
      processedFile.processingTime = Date.now() - startTime;
      
      return processedFile as ProcessedFile;
      
    } catch (error) {
      console.error(`Failed to process file ${file.name}:`, error);
      
      return {
        ...processedFile,
        processingStatus: 'failed',
        processingTime: Date.now() - startTime,
        processingErrors: [error.message],
        confidence: 0,
        textContent: '',
        summary: `Failed to process: ${error.message}`,
        keyTopics: [],
        namedEntities: [],
        embeddings: [],
        searchableText: file.name
      } as ProcessedFile;
    }
  }
  
  private createSearchableText(file: ProcessedFile): string {
    return [
      file.originalName,
      file.summary,
      file.textContent,
      file.keyTopics.join(' '),
      file.namedEntities.map(e => e.text).join(' '),
      file.tags.join(' '),
      file.categories.join(' ')
    ].filter(Boolean).join('\n');
  }
  
  private calculateProcessingConfidence(
    extractedContent: any,
    aiAnalysis: any,
    embeddingCount: number
  ): number {
    let confidence = 0.5; // Base confidence
    
    // Boost for successful text extraction
    if (extractedContent.textContent && extractedContent.textContent.length > 50) {
      confidence += 0.2;
    }
    
    // Boost for structured data extraction
    if (extractedContent.structuredData) {
      confidence += 0.1;
    }
    
    // Boost for AI analysis quality
    if (aiAnalysis.summary && aiAnalysis.summary.length > 20) {
      confidence += 0.1;
    }
    
    if (aiAnalysis.entities && aiAnalysis.entities.length > 0) {
      confidence += 0.05;
    }
    
    if (aiAnalysis.topics && aiAnalysis.topics.length > 0) {
      confidence += 0.05;
    }
    
    // Boost for successful embedding generation
    if (embeddingCount > 0) {
      confidence += 0.1;
    }
    
    return Math.min(confidence, 0.95); // Cap at 95%
  }
}

interface FileProcessorInterface {
  extractContent(file: File): Promise<{
    textContent: string;
    structuredData?: any;
    sections: ContentSection[];
    elements: ExtractedElement[];
    specificData?: any;
  }>;
}

interface ExtractedElement {
  id: string;
  type: string;
  content: any;
  position: Position;
  metadata: Record<string, any>;
}

interface Position {
  page?: number;
  line?: number;
  column?: number;
  x?: number;
  y?: number;
  width?: number;
  height?: number;
}
```

### 2.2 Advanced PDF Processor

```typescript
// src/knowledgeBase/processors/advancedPDFProcessor.ts
import * as pdfjsLib from 'pdfjs-dist';

export interface PDFData {
  pageCount: number;
  pages: PDFPageData[];
  documentInfo: PDFDocumentInfo;
  tableOfContents: TOCEntry[];
  forms: FormField[];
  annotations: PDFAnnotation[];
}

export interface PDFPageData {
  pageNumber: number;
  text: string;
  tables: TableData[];
  images: ImageData[];
  figures: FigureData[];
}

export interface TableData {
  id: string;
  rows: string[][];
  headers: string[];
  position: Position;
  confidence: number;
  metadata: {
    hasHeaders: boolean;
    dataTypes: string[];
    summary: string;
  };
}

export class AdvancedPDFProcessor implements FileProcessorInterface {
  private tableExtractor: TableExtractionService;
  private formAnalyzer: FormAnalysisService;
  
  constructor() {
    this.tableExtractor = new TableExtractionService();
    this.formAnalyzer = new FormAnalysisService();
  }
  
  async extractContent(file: File): Promise<{
    textContent: string;
    structuredData: any;
    sections: ContentSection[];
    elements: ExtractedElement[];
    specificData: PDFData;
  }> {
    const arrayBuffer = await file.arrayBuffer();
    const pdf = await pdfjsLib.getDocument({ data: arrayBuffer }).promise;
    
    // Extract document metadata
    const metadata = await pdf.getMetadata();
    const documentInfo: PDFDocumentInfo = {
      title: metadata.info.Title || '',
      author: metadata.info.Author || '',
      subject: metadata.info.Subject || '',
      creator: metadata.info.Creator || '',
      producer: metadata.info.Producer || '',
      creationDate: metadata.info.CreationDate,
      modificationDate: metadata.info.ModDate,
      pageCount: pdf.numPages
    };
    
    // Extract table of contents
    const toc = await this.extractTableOfContents(pdf);
    
    // Process each page
    const pages: PDFPageData[] = [];
    const allSections: ContentSection[] = [];
    const allElements: ExtractedElement[] = [];
    let fullText = '';
    
    for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
      const page = await pdf.getPage(pageNum);
      const pageData = await this.processPage(page, pageNum);
      
      pages.push(pageData);
      fullText += pageData.text + '\n\n';
      
      // Convert page elements to sections
      const pageSections = this.convertPageToSections(pageData, pageNum);
      allSections.push(...pageSections);
      
      // Extract page elements
      const pageElements = await this.extractPageElements(page, pageNum);
      allElements.push(...pageElements);
    }
    
    // Extract forms
    const forms = await this.formAnalyzer.extractForms(pdf);
    
    // Extract annotations
    const annotations = await this.extractAnnotations(pdf);
    
    const pdfData: PDFData = {
      pageCount: pdf.numPages,
      pages,
      documentInfo,
      tableOfContents: toc,
      forms,
      annotations
    };
    
    // Structure the document hierarchically
    const structuredSections = this.buildDocumentHierarchy(allSections, toc);
    
    return {
      textContent: fullText.trim(),
      structuredData: {
        hierarchy: structuredSections,
        tables: pages.flatMap(p => p.tables),
        forms,
        toc,
        documentInfo
      },
      sections: allSections,
      elements: allElements,
      specificData: pdfData
    };
  }
  
  private async processPage(page: any, pageNumber: number): Promise<PDFPageData> {
    // Extract text content with positioning
    const textContent = await page.getTextContent();
    const pageText = this.assembleTextWithStructure(textContent);
    
    // Extract tables using advanced table detection
    const tables = await this.tableExtractor.extractTablesFromPage(page, pageNumber);
    
    // Extract images and figures
    const images = await this.extractImagesFromPage(page, pageNumber);
    const figures = await this.extractFiguresFromPage(page, pageNumber);
    
    return {
      pageNumber,
      text: pageText,
      tables,
      images,
      figures
    };
  }
  
  private assembleTextWithStructure(textContent: any): string {
    // Group text items by position to maintain structure
    const lines: Array<{ y: number; items: any[] }> = [];
    
    textContent.items.forEach((item: any) => {
      const y = Math.round(item.transform[5]);
      let line = lines.find(l => Math.abs(l.y - y) < 5);
      
      if (!line) {
        line = { y, items: [] };
        lines.push(line);
      }
      
      line.items.push(item);
    });
    
    // Sort lines by Y position (top to bottom)
    lines.sort((a, b) => b.y - a.y);
    
    // Assemble text maintaining spatial relationships
    return lines.map(line => {
      // Sort items in line by X position (left to right)
      line.items.sort((a, b) => a.transform[4] - b.transform[4]);
      
      return line.items
        .map(item => item.str)
        .join(' ')
        .trim();
    })
    .filter(line => line.length > 0)
    .join('\n');
  }
  
  private convertPageToSections(pageData: PDFPageData, pageNumber: number): ContentSection[] {
    const sections: ContentSection[] = [];
    const lines = pageData.text.split('\n');
    
    let currentSection: Partial<ContentSection> = {};
    let sectionIndex = 0;
    
    lines.forEach((line, lineIndex) => {
      const trimmedLine = line.trim();
      if (!trimmedLine) return;
      
      // Detect headings based on formatting patterns
      const isHeading = this.detectHeading(trimmedLine, lines, lineIndex);
      
      if (isHeading) {
        // Save previous section
        if (currentSection.content) {
          sections.push({
            ...currentSection,
            id: `pdf-p${pageNumber}-s${sectionIndex++}`,
            position: { page: pageNumber, line: lineIndex },
            confidence: 0.8
          } as ContentSection);
        }
        
        // Start new heading section
        currentSection = {
          type: 'heading',
          content: trimmedLine,
          level: this.getHeadingLevel(trimmedLine),
          metadata: { 
            fontSize: 'detected',
            formatting: 'detected'
          }
        };
      } else {
        // Add to current section or create paragraph
        if (currentSection.type === 'heading') {
          // Complete the heading and start paragraph
          sections.push({
            ...currentSection,
            id: `pdf-p${pageNumber}-s${sectionIndex++}`,
            position: { page: pageNumber, line: lineIndex - 1 },
            confidence: 0.8
          } as ContentSection);
          
          currentSection = {
            type: 'paragraph',
            content: trimmedLine,
            metadata: {}
          };
        } else {
          // Continue building paragraph
          currentSection.content = (currentSection.content || '') + ' ' + trimmedLine;
        }
      }
    });
    
    // Add final section
    if (currentSection.content) {
      sections.push({
        ...currentSection,
        id: `pdf-p${pageNumber}-s${sectionIndex}`,
        position: { page: pageNumber, line: lines.length - 1 },
        confidence: 0.8
      } as ContentSection);
    }
    
    // Add table sections
    pageData.tables.forEach((table, index) => {
      sections.push({
        id: `pdf-p${pageNumber}-table${index}`,
        type: 'table',
        content: this.tableToText(table),
        position: table.position,
        metadata: {
          tableData: table,
          rowCount: table.rows.length,
          columnCount: table.headers.length
        },
        confidence: table.confidence
      });
    });
    
    return sections;
  }
  
  private detectHeading(line: string, allLines: string[], currentIndex: number): boolean {
    // Multiple heuristics for heading detection
    
    // 1. Check for common heading patterns
    const headingPatterns = [
      /^\d+\.?\s+[A-Z]/, // "1. Introduction" or "1 Introduction"
      /^[A-Z][A-Z\s\-]{5,}$/, // "INTRODUCTION" (all caps, reasonable length)
      /^[A-Z][a-z]+(\s[A-Z][a-z]+)*$/, // "Title Case Words"
    ];
    
    if (headingPatterns.some(pattern => pattern.test(line))) {
      return true;
    }
    
    // 2. Check if line is significantly shorter than average
    const avgLineLength = allLines.reduce((sum, l) => sum + l.length, 0) / allLines.length;
    const isShort = line.length < avgLineLength * 0.7 && line.length > 5;
    
    // 3. Check if next line is empty or different (indicating separation)
    const nextLineEmpty = currentIndex + 1 < allLines.length && 
                          allLines[currentIndex + 1].trim().length === 0;
    
    // 4. Check for lack of sentence-ending punctuation
    const lacksEndPunctuation = !/[.!?]$/.test(line.trim());
    
    return isShort && (nextLineEmpty || lacksEndPunctuation) && line.length < 100;
  }
  
  private getHeadingLevel(text: string): number {
    // Determine heading level based on formatting
    if (/^\d+\.\s/.test(text)) return 1; // "1. Main Section" 
    if (/^\d+\.\d+\s/.test(text)) return 2; // "1.1 Subsection"
    if (/^\d+\.\d+\.\d+\s/.test(text)) return 3; // "1.1.1 Sub-subsection"
    if (/^[A-Z][A-Z\s\-]{10,}$/.test(text)) return 1; // "MAIN SECTION"
    if (/^[A-Z][a-z]+(\s[A-Z][a-z]+)*$/.test(text)) return 2; // "Title Case"
    return 3; // Default level
  }
  
  private tableToText(table: TableData): string {
    let text = '';
    
    if (table.headers.length > 0) {
      text += table.headers.join(' | ') + '\n';
      text += table.headers.map(() => '---').join(' | ') + '\n';
    }
    
    table.rows.forEach(row => {
      text += row.join(' | ') + '\n';
    });
    
    return text;
  }
  
  private async extractTableOfContents(pdf: any): Promise<TOCEntry[]> {
    try {
      const outline = await pdf.getOutline();
      if (!outline) return [];
      
      return this.processOutlineItems(outline);
    } catch (error) {
      console.warn('Could not extract PDF outline:', error);
      return [];
    }
  }
  
  private processOutlineItems(items: any[], level: number = 1): TOCEntry[] {
    return items.map(item => ({
      title: item.title,
      page: this.getPageFromDest(item.dest),
      level,
      children: item.items ? this.processOutlineItems(item.items, level + 1) : []
    }));
  }
  
  private getPageFromDest(dest: any): number {
    if (Array.isArray(dest) && dest[0]) {
      return dest[0].num || 1;
    }
    return 1;
  }
  
  private buildDocumentHierarchy(sections: ContentSection[], toc: TOCEntry[]): any {
    // Build hierarchical structure based on headings and TOC
    const hierarchy: any = {
      title: 'Document',
      sections: [],
      toc
    };
    
    let currentLevel1: any = null;
    let currentLevel2: any = null;
    
    sections.forEach(section => {
      if (section.type === 'heading') {
        const level = section.level || 1;
        
        if (level === 1) {
          currentLevel1 = {
            title: section.content,
            level: 1,
            sections: [],
            content: []
          };
          hierarchy.sections.push(currentLevel1);
          currentLevel2 = null;
        } else if (level === 2 && currentLevel1) {
          currentLevel2 = {
            title: section.content,
            level: 2,
            sections: [],
            content: []
          };
          currentLevel1.sections.push(currentLevel2);
        }
      } else {
        // Add content to appropriate level
        const target = currentLevel2 || currentLevel1 || hierarchy;
        if (target && target.content) {
          target.content.push(section);
        }
      }
    });
    
    return hierarchy;
  }
  
  // Additional helper methods...
  private async extractPageElements(page: any, pageNumber: number): Promise<ExtractedElement[]> {
    const elements: ExtractedElement[] = [];
    
    try {
      const annotations = await page.getAnnotations();
      
      annotations.forEach((annotation: any, index: number) => {
        elements.push({
          id: `pdf-p${pageNumber}-annotation${index}`,
          type: 'annotation',
          content: annotation,
          position: {
            page: pageNumber,
            x: annotation.rect[0],
            y: annotation.rect[1],
            width: annotation.rect[2] - annotation.rect[0],
            height: annotation.rect[3] - annotation.rect[1]
          },
          metadata: {
            subtype: annotation.subtype,
            contents: annotation.contents
          }
        });
      });
    } catch (error) {
      console.warn('Could not extract page annotations:', error);
    }
    
    return elements;
  }
  
  private async extractImagesFromPage(page: any, pageNumber: number): Promise<ImageData[]> {
    // Implementation for image extraction
    return [];
  }
  
  private async extractFiguresFromPage(page: any, pageNumber: number): Promise<FigureData[]> {
    // Implementation for figure extraction
    return [];
  }
  
  private async extractAnnotations(pdf: any): Promise<PDFAnnotation[]> {
    const annotations: PDFAnnotation[] = [];
    
    for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
      const page = await pdf.getPage(pageNum);
      const pageAnnotations = await page.getAnnotations();
      
      pageAnnotations.forEach((annotation: any) => {
        annotations.push({
          id: `annotation-p${pageNum}-${annotation.id}`,
          page: pageNum,
          type: annotation.subtype,
          content: annotation.contents || '',
          rect: annotation.rect,
          author: annotation.title || '',
          modificationDate: annotation.modificationDate
        });
      });
    }
    
    return annotations;
  }
}

// Supporting interfaces and services
interface PDFDocumentInfo {
  title: string;
  author: string;
  subject: string;
  creator: string;
  producer: string;
  creationDate?: string;
  modificationDate?: string;
  pageCount: number;
}

interface TOCEntry {
  title: string;
  page: number;
  level: number;
  children: TOCEntry[];
}

interface FormField {
  name: string;
  type: string;
  value: any;
  required: boolean;
  page: number;
}

interface PDFAnnotation {
  id: string;
  page: number;
  type: string;
  content: string;
  rect: number[];
  author: string;
  modificationDate?: string;
}

interface ImageData {
  id: string;
  page: number;
  position: Position;
  alt?: string;
}

interface FigureData {
  id: string;
  page: number;
  caption?: string;
  position: Position;
}

class TableExtractionService {
  async extractTablesFromPage(page: any, pageNumber: number): Promise<TableData[]> {
    // Advanced table extraction implementation
    return [];
  }
}

class FormAnalysisService {
  async extractForms(pdf: any): Promise<FormField[]> {
    // Form field extraction implementation  
    return [];
  }
}
```

### 2.3 Intelligent Image Processor with Vision AI

```typescript
// src/knowledgeBase/processors/intelligentImageProcessor.ts
export interface ImageData {
  dimensions: { width: number; height: number };
  format: string;
  colorProfile: ColorProfile;
  
  // AI Vision Analysis
  ocrText: string;
  visualDescription: string;
  detectedObjects: DetectedObject[];
  sceneClassification: SceneClassification;
  
  // UI/UX Analysis (for mockups, wireframes)
  uiAnalysis?: UIAnalysis;
  
  // Technical Analysis
  technicalSpecs: ImageTechnicalSpecs;
  qualityMetrics: ImageQualityMetrics;
}

export interface ColorProfile {
  dominantColors: ColorInfo[];
  colorSpace: string;
  hasTransparency: boolean;
  averageBrightness: number;
  contrast: number;
}

export interface ColorInfo {
  hex: string;
  rgb: [number, number, number];
  hsl: [number, number, number];
  percentage: number;
  name: string;
  palette: 'primary' | 'secondary' | 'accent' | 'neutral';
}

export interface DetectedObject {
  id: string;
  label: string;
  confidence: number;
  boundingBox: BoundingBox;
  attributes: Record<string, any>;
  category: ObjectCategory;
}

export interface SceneClassification {
  primaryScene: string;
  confidence: number;
  tags: string[];
  context: SceneContext;
  complexity: 'simple' | 'moderate' | 'complex';
}

export interface UIAnalysis {
  isUI: boolean;
  confidence: number;
  uiType: 'mockup' | 'wireframe' | 'screenshot' | 'prototype';
  elements: UIElement[];
  layout: LayoutAnalysis;
  designSystem: DesignSystemAnalysis;
}

export interface UIElement {
  id: string;
  type: UIElementType;
  text?: string;
  position: BoundingBox;
  confidence: number;
  style?: ElementStyle;
  interactions?: InteractionHint[];
}

export interface LayoutAnalysis {
  structure: 'grid' | 'flexbox' | 'absolute' | 'flow';
  regions: LayoutRegion[];
  hierarchy: LayoutHierarchy;
  responsive: boolean;
}

export interface DesignSystemAnalysis {
  colorPalette: ColorInfo[];
  typography: TypographyInfo[];
  spacing: SpacingInfo;
  components: ComponentAnalysis[];
}

export class IntelligentImageProcessor implements FileProcessorInterface {
  private visionAI: VisionAIService;
  private ocrService: OCRService;
  private colorAnalyzer: ColorAnalyzerService;
  private uiAnalyzer: UIAnalysisService;
  
  constructor() {
    this.visionAI = new VisionAIService();
    this.ocrService = new OCRService();
    this.colorAnalyzer = new ColorAnalyzerService();
    this.uiAnalyzer = new UIAnalysisService();
  }
  
  async extractContent(file: File): Promise<{
    textContent: string;
    structuredData: any;
    sections: ContentSection[];
    elements: ExtractedElement[];
    specificData: ImageData;
  }> {
    const imageBuffer = await file.arrayBuffer();
    
    // Parallel processing for efficiency
    const [
      basicAnalysis,
      ocrResult,
      visionAnalysis,
      colorAnalysis,
      uiAnalysis
    ] = await Promise.all([
      this.analyzeImageBasics(imageBuffer, file.type),
      this.ocrService.extractText(imageBuffer),
      this.visionAI.analyzeImage(imageBuffer),
      this.colorAnalyzer.analyzeColors(imageBuffer),
      this.uiAnalyzer.analyzeUIElements(imageBuffer)
    ]);
    
    const imageData: ImageData = {
      dimensions: basicAnalysis.dimensions,
      format: file.type,
      colorProfile: colorAnalysis,
      ocrText: ocrResult.text,
      visualDescription: visionAnalysis.description,
      detectedObjects: visionAnalysis.objects,
      sceneClassification: visionAnalysis.scene,
      uiAnalysis: uiAnalysis.isUI ? uiAnalysis : undefined,
      technicalSpecs: basicAnalysis.specs,
      qualityMetrics: basicAnalysis.quality
    };
    
    // Create content sections from different analysis results
    const sections = this.createImageContentSections(imageData);
    
    // Create extracted elements
    const elements = this.createImageElements(imageData);
    
    // Combine all textual content
    const textContent = this.assembleTextContent(imageData);
    
    return {
      textContent,
      structuredData: {
        visual: visionAnalysis,
        ui: uiAnalysis,
        color: colorAnalysis,
        technical: basicAnalysis
      },
      sections,
      elements,
      specificData: imageData
    };
  }
  
  private async analyzeImageBasics(buffer: ArrayBuffer, mimeType: string): Promise<{
    dimensions: { width: number; height: number };
    specs: ImageTechnicalSpecs;
    quality: ImageQualityMetrics;
  }> {
    // Would integrate with image processing library like Sharp
    const imageInfo = await this.getImageInfo(buffer, mimeType);
    
    return {
      dimensions: imageInfo.dimensions,
      specs: {
        dpi: imageInfo.density || 72,
        bitDepth: imageInfo.channels * 8,
        compression: imageInfo.compression,
        fileSize: buffer.byteLength,
        metadata: imageInfo.exif || {}
      },
      quality: {
        sharpness: imageInfo.sharpness || 0.8,
        noise: imageInfo.noise || 0.2,
        exposure: imageInfo.exposure || 0.5,
        overall: imageInfo.quality || 0.8
      }
    };
  }
  
  private createImageContentSections(imageData: ImageData): ContentSection[] {
    const sections: ContentSection[] = [];
    
    // OCR Text Section
    if (imageData.ocrText && imageData.ocrText.trim()) {
      sections.push({
        id: 'image-ocr-text',
        type: 'paragraph',
        content: imageData.ocrText,
        position: { line: 0 },
        metadata: {
          source: 'ocr',
          confidence: 0.85,
          language: this.detectLanguage(imageData.ocrText)
        },
        confidence: 0.85
      });
    }
    
    // Visual Description Section
    if (imageData.visualDescription) {
      sections.push({
        id: 'image-description',
        type: 'paragraph',
        content: imageData.visualDescription,
        position: { line: 1 },
        metadata: {
          source: 'vision-ai',
          scene: imageData.sceneClassification.primaryScene,
          tags: imageData.sceneClassification.tags
        },
        confidence: imageData.sceneClassification.confidence
      });
    }
    
    // Objects List Section
    if (imageData.detectedObjects.length > 0) {
      const objectsText = this.createObjectsDescription(imageData.detectedObjects);
      sections.push({
        id: 'image-objects',
        type: 'list',
        content: objectsText,
        position: { line: 2 },
        metadata: {
          source: 'object-detection',
          objectCount: imageData.detectedObjects.length
        },
        confidence: 0.8
      });
    }
    
    // UI Analysis Section
    if (imageData.uiAnalysis && imageData.uiAnalysis.isUI) {
      const uiDescription = this.createUIDescription(imageData.uiAnalysis);
      sections.push({
        id: 'image-ui-analysis',
        type: 'paragraph',
        content: uiDescription,
        position: { line: 3 },
        metadata: {
          source: 'ui-analysis',
          uiType: imageData.uiAnalysis.uiType,
          elementCount: imageData.uiAnalysis.elements.length
        },
        confidence: imageData.uiAnalysis.confidence
      });
    }
    
    // Color Analysis Section
    const colorDescription = this.createColorDescription(imageData.colorProfile);
    sections.push({
      id: 'image-colors',
      type: 'paragraph',
      content: colorDescription,
      position: { line: 4 },
      metadata: {
        source: 'color-analysis',
        dominantColor: imageData.colorProfile.dominantColors[0]?.name || 'unknown'
      },
      confidence: 0.9
    });
    
    return sections;
  }
  
  private createObjectsDescription(objects: DetectedObject[]): string {
    const highConfidenceObjects = objects.filter(obj => obj.confidence > 0.7);
    
    if (highConfidenceObjects.length === 0) {
      return 'No distinct objects detected with high confidence.';
    }
    
    const grouped = this.groupObjectsByCategory(highConfidenceObjects);
    const descriptions: string[] = [];
    
    Object.entries(grouped).forEach(([category, objs]) => {
      const labels = objs.map(obj => obj.label).join(', ');
      descriptions.push(`${category}: ${labels}`);
    });
    
    return `Detected objects include: ${descriptions.join('; ')}`;
  }
  
  private createUIDescription(uiAnalysis: UIAnalysis): string {
    const elementCounts = this.countUIElements(uiAnalysis.elements);
    const elementDescriptions = Object.entries(elementCounts)
      .map(([type, count]) => `${count} ${type}${count > 1 ? 's' : ''}`)
      .join(', ');
    
    let description = `${uiAnalysis.uiType.charAt(0).toUpperCase() + uiAnalysis.uiType.slice(1)} containing: ${elementDescriptions}`;
    
    if (uiAnalysis.layout) {
      description += `. Layout structure: ${uiAnalysis.layout.structure}`;
    }
    
    if (uiAnalysis.designSystem) {
      description += `. Design system includes ${uiAnalysis.designSystem.colorPalette.length} colors and ${uiAnalysis.designSystem.components.length} component types`;
    }
    
    return description;
  }
  
  private createColorDescription(colorProfile: ColorProfile): string {
    const dominantColors = colorProfile.dominantColors.slice(0, 3);
    const colorNames = dominantColors.map(color => 
      `${color.name} (${color.percentage.toFixed(1)}%)`
    ).join(', ');
    
    let description = `Color palette: ${colorNames}`;
    
    if (colorProfile.hasTransparency) {
      description += '. Image has transparency';
    }
    
    description += `. Overall brightness: ${Math.round(colorProfile.averageBrightness * 100)}%, contrast: ${Math.round(colorProfile.contrast * 100)}%`;
    
    return description;
  }
  
  private createImageElements(imageData: ImageData): ExtractedElement[] {
    const elements: ExtractedElement[] = [];
    
    // Object elements
    imageData.detectedObjects.forEach((object, index) => {
      elements.push({
        id: `image-object-${index}`,
        type: 'detected-object',
        content: object,
        position: {
          x: object.boundingBox.x,
          y: object.boundingBox.y,
          width: object.boundingBox.width,
          height: object.boundingBox.height
        },
        metadata: {
          label: object.label,
          confidence: object.confidence,
          category: object.category
        }
      });
    });
    
    // UI elements
    if (imageData.uiAnalysis?.elements) {
      imageData.uiAnalysis.elements.forEach((uiElement, index) => {
        elements.push({
          id: `image-ui-element-${index}`,
          type: 'ui-element',
          content: uiElement,
          position: {
            x: uiElement.position.x,
            y: uiElement.position.y,
            width: uiElement.position.width,
            height: uiElement.position.height
          },
          metadata: {
            elementType: uiElement.type,
            text: uiElement.text,
            confidence: uiElement.confidence
          }
        });
      });
    }
    
    return elements;
  }
  
  private assembleTextContent(imageData: ImageData): string {
    const textParts: string[] = [];
    
    // Add OCR text
    if (imageData.ocrText?.trim()) {
      textParts.push(imageData.ocrText);
    }
    
    // Add visual description
    if (imageData.visualDescription) {
      textParts.push(imageData.visualDescription);
    }
    
    // Add object labels
    const objectLabels = imageData.detectedObjects
      .filter(obj => obj.confidence > 0.7)
      .map(obj => obj.label);
    if (objectLabels.length > 0) {
      textParts.push(`Contains: ${objectLabels.join(', ')}`);
    }
    
    // Add UI element text
    if (imageData.uiAnalysis?.elements) {
      const uiTexts = imageData.uiAnalysis.elements
        .map(el => el.text)
        .filter(Boolean);
      if (uiTexts.length > 0) {
        textParts.push(`UI text: ${uiTexts.join(' ')}`);
      }
    }
    
    // Add scene tags
    if (imageData.sceneClassification.tags.length > 0) {
      textParts.push(`Scene tags: ${imageData.sceneClassification.tags.join(', ')}`);
    }
    
    return textParts.join('\n\n');
  }
  
  // Helper methods
  private detectLanguage(text: string): string {
    // Simple language detection
    if (!text || text.length < 10) return 'unknown';
    
    const hasLatinChars = /[a-zA-Z]/.test(text);
    const hasCJKChars = /[\u4e00-\u9fff\u3400-\u4dbf\u3040-\u309f\u30a0-\u30ff]/.test(text);
    
    if (hasCJKChars) return 'zh';
    if (hasLatinChars) return 'en';
    return 'unknown';
  }
  
  private groupObjectsByCategory(objects: DetectedObject[]): Record<string, DetectedObject[]> {
    const grouped: Record<string, DetectedObject[]> = {};
    
    objects.forEach(obj => {
      const category = obj.category || 'other';
      if (!grouped[category]) {
        grouped[category] = [];
      }
      grouped[category].push(obj);
    });
    
    return grouped;
  }
  
  private countUIElements(elements: UIElement[]): Record<string, number> {
    const counts: Record<string, number> = {};
    
    elements.forEach(element => {
      counts[element.type] = (counts[element.type] || 0) + 1;
    });
    
    return counts;
  }
  
  private async getImageInfo(buffer: ArrayBuffer, mimeType: string): Promise<any> {
    // Placeholder - would use actual image processing library
    return {
      dimensions: { width: 1024, height: 768 },
      density: 72,
      channels: 3,
      compression: 'none',
      quality: 0.8,
      sharpness: 0.8,
      noise: 0.2,
      exposure: 0.5,
      exif: {}
    };
  }
}

// Supporting types and interfaces
type UIElementType = 'button' | 'input' | 'text' | 'image' | 'icon' | 'menu' | 'card' | 'modal' | 'form' | 'navigation';
type ObjectCategory = 'person' | 'object' | 'vehicle' | 'animal' | 'food' | 'technology' | 'furniture' | 'other';
type SceneContext = 'indoor' | 'outdoor' | 'office' | 'home' | 'public' | 'digital' | 'nature' | 'urban';

interface BoundingBox {
  x: number;
  y: number;
  width: number;
  height: number;
}

interface ElementStyle {
  backgroundColor?: string;
  textColor?: string;
  borderColor?: string;
  fontSize?: string;
  fontFamily?: string;
}

interface InteractionHint {
  type: 'clickable' | 'hoverable' | 'draggable' | 'scrollable';
  confidence: number;
}

interface LayoutRegion {
  name: string;
  type: 'header' | 'sidebar' | 'content' | 'footer' | 'navigation';
  position: BoundingBox;
}

interface LayoutHierarchy {
  depth: number;
  parents: string[];
  children: string[];
}

interface TypographyInfo {
  fontFamily: string;
  fontSize: number;
  weight: string;
  usage: 'heading' | 'body' | 'caption' | 'label';
}

interface SpacingInfo {
  margins: number[];
  paddings: number[];
  gaps: number[];
  grid: { columns: number; rows: number; gap: number } | null;
}

interface ComponentAnalysis {
  type: string;
  count: number;
  variations: string[];
  consistency: number;
}

interface ImageTechnicalSpecs {
  dpi: number;
  bitDepth: number;
  compression?: string;
  fileSize: number;
  metadata: Record<string, any>;
}

interface ImageQualityMetrics {
  sharpness: number;
  noise: number;
  exposure: number;
  overall: number;
}

// Supporting services (would be implemented with actual AI APIs)
class VisionAIService {
  async analyzeImage(buffer: ArrayBuffer): Promise<{
    description: string;
    objects: DetectedObject[];
    scene: SceneClassification;
  }> {
    // Integration with Google Vision API, AWS Rekognition, etc.
    return {
      description: '',
      objects: [],
      scene: { primaryScene: '', confidence: 0, tags: [], context: 'digital', complexity: 'simple' }
    };
  }
}

class OCRService {
  async extractText(buffer: ArrayBuffer): Promise<{ text: string; confidence: number }> {
    // Integration with Tesseract, Google Vision API, etc.
    return { text: '', confidence: 0 };
  }
}

class ColorAnalyzerService {
  async analyzeColors(buffer: ArrayBuffer): Promise<ColorProfile> {
    // Color analysis implementation
    return {
      dominantColors: [],
      colorSpace: 'RGB',
      hasTransparency: false,
      averageBrightness: 0.5,
      contrast: 0.5
    };
  }
}

class UIAnalysisService {
  async analyzeUIElements(buffer: ArrayBuffer): Promise<UIAnalysis> {
    // UI element detection implementation
    return {
      isUI: false,
      confidence: 0,
      uiType: 'mockup',
      elements: [],
      layout: {
        structure: 'flow',
        regions: [],
        hierarchy: { depth: 0, parents: [], children: [] },
        responsive: false
      },
      designSystem: {
        colorPalette: [],
        typography: [],
        spacing: { margins: [], paddings: [], gaps: [], grid: null },
        components: []
      }
    };
  }
}
```

## 3. Knowledge Base Integration in VS Code

### 3.1 VS Code Knowledge Base Panel

```typescript
// extensions/pronetheia-ai/src/views/knowledgeBaseView.ts
import * as vscode from 'vscode';
import { KnowledgeBaseManager } from '../knowledgeBase/knowledgeBaseManager';
import { ProcessedFile } from '../knowledgeBase/fileProcessor';

export class KnowledgeBaseViewProvider implements vscode.WebviewViewProvider {
  public static readonly viewType = 'pronetheiaKnowledgeBase';
  private _view?: vscode.WebviewView;
  private knowledgeBase: KnowledgeBaseManager;
  
  constructor(
    private readonly _extensionUri: vscode.Uri,
    knowledgeBase: KnowledgeBaseManager
  ) {
    this.knowledgeBase = knowledgeBase;
  }
  
  public resolveWebviewView(
    webviewView: vscode.WebviewView,
    context: vscode.WebviewViewResolveContext,
    _token: vscode.CancellationToken,
  ) {
    this._view = webviewView;
    
    webviewView.webview.options = {
      enableScripts: true,
      localResourceRoots: [this._extensionUri]
    };
    
    webviewView.webview.html = this._getHtmlForWebview(webviewView.webview);
    
    // Handle messages from React app
    webviewView.webview.onDidReceiveMessage(async (data) => {
      switch (data.type) {
        case 'uploadFiles':
          await this.handleFileUpload(data.files);
          break;
        case 'searchFiles':
          await this.handleFileSearch(data.query);
          break;
        case 'deleteFile':
          await this.handleFileDelete(data.fileId);
          break;
        case 'getFileDetails':
          await this.handleGetFileDetails(data.fileId);
          break;
        case 'addToContext':
          await this.handleAddToContext(data.fileIds);
          break;
      }
    });
    
    // Load initial data
    this.loadInitialData();
  }
  
  private async handleFileUpload(files: { name: string; content: string; type: string }[]): Promise<void> {
    try {
      const uploadPromises = files.map(async (fileData) => {
        // Convert base64 content back to File object
        const buffer = Buffer.from(fileData.content, 'base64');
        const file = new File([buffer], fileData.name, { type: fileData.type });
        
        // Get current workspace
        const workspaceFolder = vscode.workspace.workspaceFolders?.[0];
        const projectId = workspaceFolder?.uri.fsPath || 'default';
        const userId = 'current-user'; // Would get from auth
        
        // Process the file
        return await this.knowledgeBase.addFile(file, userId, projectId);
      });
      
      const results = await Promise.all(uploadPromises);
      
      // Send results back to webview
      this._view?.webview.postMessage({
        type: 'fileUploadResults',
        results: results.map(result => ({
          success: result.processingStatus === 'completed',
          file: result,
          error: result.processingErrors?.[0]
        }))
      });
      
      // Refresh the file list
      await this.refreshFileList();
      
    } catch (error) {
      vscode.window.showErrorMessage(`Failed to upload files: ${error.message}`);
    }
  }
  
  private async handleFileSearch(query: string): Promise<void> {
    try {
      const searchResults = await this.knowledgeBase.searchFiles({
        text: query,
        pagination: { offset: 0, limit: 20 }
      });
      
      this._view?.webview.postMessage({
        type: 'searchResults',
        results: searchResults
      });
      
    } catch (error) {
      vscode.window.showErrorMessage(`Search failed: ${error.message}`);
    }
  }
  
  private async refreshFileList(): Promise<void> {
    try {
      const workspaceFolder = vscode.workspace.workspaceFolders?.[0];
      const projectId = workspaceFolder?.uri.fsPath || 'default';
      
      const files = await this.knowledgeBase.getProjectFiles(projectId);
      
      this._view?.webview.postMessage({
        type: 'fileListUpdate',
        files: files
      });
      
    } catch (error) {
      console.error('Failed to refresh file list:', error);
    }
  }
  
  private async loadInitialData(): Promise<void> {
    await this.refreshFileList();
    
    // Load knowledge base statistics
    const stats = await this.knowledgeBase.getStatistics();
    this._view?.webview.postMessage({
      type: 'statsUpdate',
      stats
    });
  }
  
  private _getHtmlForWebview(webview: vscode.Webview): string {
    const scriptUri = webview.asWebviewUri(vscode.Uri.joinPath(this._extensionUri, 'media', 'knowledgeBase.js'));
    const styleUri = webview.asWebviewUri(vscode.Uri.joinPath(this._extensionUri, 'media', 'knowledgeBase.css'));
    
    return `<!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Pronetheia Knowledge Base</title>
        <link href="${styleUri}" rel="stylesheet">
      </head>
      <body>
        <div id="knowledge-base-root"></div>
        <script src="${scriptUri}"></script>
      </body>
      </html>`;
  }
}
```

### 3.2 React Knowledge Base Interface

```typescript
// extensions/pronetheia-ai/media/src/components/KnowledgeBasePanel.tsx
import React, { useState, useEffect, useCallback } from 'react';
import { ProcessedFile, SearchResult } from '../types';
import { FileUploadZone } from './FileUploadZone';
import { FileList } from './FileList';
import { FileSearchBar } from './FileSearchBar';
import { FileDetailsModal } from './FileDetailsModal';
import { KnowledgeBaseStats } from './KnowledgeBaseStats';

export const KnowledgeBasePanel: React.FC = () => {
  const [files, setFiles] = useState<ProcessedFile[]>([]);
  const [searchResults, setSearchResults] = useState<SearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [selectedFile, setSelectedFile] = useState<ProcessedFile | null>(null);
  const [stats, setStats] = useState<any>(null);
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [filterCategory, setFilterCategory] = useState<string>('all');
  
  // Listen for messages from VS Code extension
  useEffect(() => {
    const messageHandler = (event: MessageEvent) => {
      const { type, data } = event.data;
      
      switch (type) {
        case 'fileListUpdate':
          setFiles(data.files);
          break;
        case 'searchResults':
          setSearchResults(data.results);
          setIsSearching(false);
          break;
        case 'fileUploadResults':
          handleUploadResults(data.results);
          break;
        case 'statsUpdate':
          setStats(data.stats);
          break;
      }
    };
    
    window.addEventListener('message', messageHandler);
    return () => window.removeEventListener('message', messageHandler);
  }, []);
  
  const handleFileUpload = useCallback(async (uploadedFiles: File[]) => {
    const fileData = await Promise.all(
      uploadedFiles.map(async (file) => ({
        name: file.name,
        type: file.type,
        content: await fileToBase64(file)
      }))
    );
    
    vscode.postMessage({
      type: 'uploadFiles',
      files: fileData
    });
  }, []);
  
  const handleSearch = useCallback((query: string) => {
    if (!query.trim()) {
      setSearchResults([]);
      return;
    }
    
    setIsSearching(true);
    vscode.postMessage({
      type: 'searchFiles',
      query: query.trim()
    });
  }, []);
  
  const handleFileSelect = useCallback((file: ProcessedFile) => {
    setSelectedFile(file);
  }, []);
  
  const handleAddToContext = useCallback((fileIds: string[]) => {
    vscode.postMessage({
      type: 'addToContext',
      fileIds
    });
    
    // Show success message
    vscode.postMessage({
      type: 'showMessage',
      message: `Added ${fileIds.length} file(s) to AI context`
    });
  }, []);
  
  const filteredFiles = files.filter(file => {
    if (filterCategory === 'all') return true;
    return file.categories.includes(filterCategory);
  });
  
  const displayFiles = searchResults.length > 0 
    ? searchResults.map(result => result.file)
    : filteredFiles;
  
  return (
    <div className="knowledge-base-panel" style={{
      height: '100%',
      display: 'flex',
      flexDirection: 'column',
      background: 'var(--vscode-editor-background)',
      color: 'var(--vscode-foreground)'
    }}>
      {/* Header */}
      <div className="kb-header" style={{
        padding: '12px 16px',
        borderBottom: '1px solid var(--vscode-widget-border)'
      }}>
        <div style={{ 
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'center',
          marginBottom: '8px'
        }}>
          <h2 style={{ margin: 0, fontSize: '16px' }}>Knowledge Base</h2>
          
          <div style={{ display: 'flex', gap: '4px'