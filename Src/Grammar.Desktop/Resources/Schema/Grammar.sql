CREATE TABLE Meta
(
	[Schema] nvarchar(100) NOT NULL,
	Major int DEFAULT 1 NOT NULL,
	Minor int DEFAULT 0 NOT NULL,
	CONSTRAINT PkSchema PRIMARY KEY ([Schema])
);

INSERT INTO Meta ([Schema], Major, Minor) VALUES ('LinguaSpace.Grammar', 1, 0);

CREATE TABLE Grammar
(
	GrammarId uniqueidentifier ROWGUIDCOL NOT NULL,
	Title nvarchar(100) NOT NULL,
	Comment nvarchar(1000) NOT NULL,
	TargetLang nvarchar(8) NOT NULL,
	NativeLang nvarchar(8) NOT NULL,
	Shuffle bit DEFAULT 0 NOT NULL,
	ShowRule bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkGrammar PRIMARY KEY (GrammarId)
);

CREATE TABLE Topics
(
	TopicId uniqueidentifier ROWGUIDCOL NOT NULL,
	ParentTopicId uniqueidentifier,
	Position smallint NOT NULL,
	Title nvarchar(200) NOT NULL,
	Expanded bit DEFAULT 0 NOT NULL,
	CONSTRAINT PkTopics PRIMARY KEY (TopicId),
	CONSTRAINT FkTopicsTopics FOREIGN KEY (ParentTopicId) REFERENCES Topics (TopicId)
);

CREATE INDEX Idx_Topics ON Topics (ParentTopicId, Position);

CREATE TABLE Rules
(
	RuleId uniqueidentifier ROWGUIDCOL NOT NULL,
	TopicId uniqueidentifier NOT NULL,
	Position smallint NOT NULL,
	Comment nvarchar(3000) NOT NULL,
	Active bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkRules PRIMARY KEY (RuleId),
	CONSTRAINT FkRulesTopics FOREIGN KEY (TopicId) REFERENCES Topics (TopicId)
);

CREATE INDEX Idx_Rules ON Topics (TopicId, Position);

CREATE TABLE Examples
(
	ExampleId uniqueidentifier ROWGUIDCOL NOT NULL,
	RuleId uniqueidentifier NOT NULL,
	Position smallint NOT NULL,
	TargetText nvarchar(2000) NOT NULL,
	NativeText nvarchar(2000) NOT NULL,
	Active bit DEFAULT 1 NOT NULL,
	Exception bit DEFAULT 0 NOT NULL,
	CONSTRAINT PkExamples PRIMARY KEY (ExampleId),
	CONSTRAINT FkExamplesRules FOREIGN KEY (RuleId) REFERENCES Rules (RuleId)
);

CREATE INDEX Idx_Examples ON Examples (RuleId, Position);
