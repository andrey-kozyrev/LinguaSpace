CREATE TABLE Meta
(
	[Schema] nvarchar(100) NOT NULL,
	Major int DEFAULT 1 NOT NULL,
	Minor int DEFAULT 0 NOT NULL,
	CONSTRAINT PkSchema PRIMARY KEY ([Schema])
);

INSERT INTO Meta ([Schema], Major, Minor) VALUES ('LinguaSpace.Words.Vocabulary', 1, 0);

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

CREATE INDEX Idx_Types ON Types (Position);

CREATE TABLE Categories
(
	CategoryId uniqueidentifier ROWGUIDCOL NOT NULL,
	Position smallint NOT NULL,
	Category nvarchar(100) NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkTypes PRIMARY KEY (CategoryId)
);

CREATE INDEX Idx_Categories ON Categories (Position);

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

CREATE INDEX Idx_Words ON Words (Word, TypeId);
CREATE INDEX Idx_Words_TypeId ON Words (TypeId);

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

CREATE INDEX Idx_Meanings ON Meanings (WordId, Position);
CREATE INDEX Idx_Meanings_WordId ON Meanings (WordId);
CREATE INDEX Idx_Meanings_SyncMod ON Meanings (SyncMod DESC);
CREATE INDEX Idx_Meanings_Definition ON Meanings (Definition);

CREATE TABLE MeaningsCategories
(
	MeaningCategoryId uniqueidentifier ROWGUIDCOL NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	CategoryId uniqueidentifier NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkMeaningsCategories PRIMARY KEY (MeaningCategoryId),
	CONSTRAINT FkCategoriesMeanings FOREIGN KEY (MeaningId) REFERENCES Meanings (MeaningId),
	CONSTRAINT FkMeaningsCategories FOREIGN KEY (CategoryId) REFERENCES Categories (CategoryId)
);

CREATE TABLE Translations
(
	TranslationId uniqueidentifier ROWGUIDCOL NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	Position smallint NOT NULL,
	Translation nvarchar(200) NOT NULL,	
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkTranslations PRIMARY KEY (TranslationId),
	CONSTRAINT FkTranslationsMeanings FOREIGN KEY (MeaningId) REFERENCES Meanings (MeaningId)
);

CREATE INDEX Idx_Translations ON Translations (MeaningId, Position);
CREATE INDEX Idx_Translations_MeaningId ON Translations (MeaningId);

CREATE TABLE Relations
(
	RelationId uniqueidentifier ROWGUIDCOL NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	Relation nvarchar(1) NOT NULL,
	Position smallint NOT NULL,
	WordId uniqueidentifier NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	SyncLive bit DEFAULT 1 NOT NULL,
	CONSTRAINT PkRelations PRIMARY KEY (RelationId),
	CONSTRAINT FkRelationsMeanings FOREIGN KEY (MeaningId) REFERENCES Meanings (MeaningId),
	CONSTRAINT FkRelationsWords FOREIGN KEY (WordId) REFERENCES Words (WordId)
);

CREATE INDEX Idx_Relations ON Relations (MeaningId, Relation, Position);
CREATE INDEX Idx_Relations_MeaningId ON Relations (MeaningId);
CREATE INDEX Idx_Relations_WordId ON Relations (WordId);