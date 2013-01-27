CREATE TABLE Vocabularies
(
	VocabularyId uniqueidentifier ROWGUIDCOL NOT NULL,
	Name nvarchar(100) NOT NULL,
	Description nvarchar(1000) NOT NULL,
	TargetLang nvarchar(8) NOT NULL,
	NativeLang nvarchar(8) NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	CONSTRAINT PkVocabularies PRIMARY KEY (VocabularyId)
);

CREATE TABLE Types
(
	TypeId uniqueidentifier ROWGUIDCOL NOT NULL,
	Position smallint NOT NULL,
	[Type] nvarchar(100) NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkTypes PRIMARY KEY (TypeId)
);

CREATE TABLE Words
(
	WordId uniqueidentifier ROWGUIDCOL NOT NULL,
	TypeId uniqueidentifier NOT NULL,
	Prefix nvarchar(20) NOT NULL,
	Word nvarchar(200) NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkWords PRIMARY KEY (WordId),
	CONSTRAINT FkWordsTypes FOREIGN KEY (TypeId) REFERENCES Types (TypeId)
);

CREATE TABLE Meanings
(
	MeaningId uniqueidentifier ROWGUIDCOL NOT NULL,
	Position smallint NOT NULL,
	Prefix nvarchar(100),
	WordId uniqueidentifier NOT NULL,
	Postfix nvarchar(100),
	Definition nvarchar(1000),
	Example nvarchar(1000),
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkMeanings PRIMARY KEY (MeaningId),
	CONSTRAINT FkMeaningsWords FOREIGN KEY (WordId) REFERENCES Words (WordId)
);

CREATE TABLE Translations
(
	TranslationId uniqueidentifier ROWGUIDCOL NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	Position smallint NOT NULL,
	Text nvarchar(200) NOT NULL,	
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkTranslations PRIMARY KEY (TranslationId),
	CONSTRAINT FkTranslationsMeanings FOREIGN KEY (MeaningId) REFERENCES Meanings (MeaningId)
);

CREATE TABLE Synonyms
(
	SynonymId uniqueidentifier ROWGUIDCOL NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	Position smallint NOT NULL,
	WordId uniqueidentifier NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkSynonyms PRIMARY KEY (SynonymId),
	CONSTRAINT FkSynonymsMeanings FOREIGN KEY (MeaningId) REFERENCES Meanings (MeaningId),
	CONSTRAINT FkSynonymsWords FOREIGN KEY (WordId) REFERENCES Words (WordId)
);

CREATE TABLE Antonyms
(
	AntonymId uniqueidentifier ROWGUIDCOL NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	Position smallint NOT NULL,
	WordId uniqueidentifier NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkAntonyms PRIMARY KEY (AntonymId),
	CONSTRAINT FkAntonymsMeanings FOREIGN KEY (MeaningId) REFERENCES Meanings (MeaningId),
	CONSTRAINT FkAntonymsWords FOREIGN KEY (WordId) REFERENCES Words (WordId)
);

CREATE INDEX IdxTypes ON Types (Text);

CREATE INDEX IdxWords ON Words (Text, TypeId, Prefix);
