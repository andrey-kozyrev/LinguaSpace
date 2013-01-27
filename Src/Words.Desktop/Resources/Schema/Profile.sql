CREATE TABLE Meta
(
	[Schema] nvarchar(100) NOT NULL,
	Major int DEFAULT 1 NOT NULL,
	Minor int DEFAULT 0 NOT NULL,
	CONSTRAINT PkSchema PRIMARY KEY ([Schema])
);

INSERT INTO Meta ([Schema], Major, Minor) VALUES ('LinguaSpace.Words.Profile', 1, 0);

CREATE TABLE Sync
(
	SyncId uniqueidentifier ROWGUIDCOL NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	CONSTRAINT PkSync PRIMARY KEY (SyncId)
);

CREATE TABLE Profiles
(
	ProfileId uniqueidentifier ROWGUIDCOL NOT NULL,
	Name nvarchar(100) NOT NULL,
	Description nvarchar(1000) NOT NULL,
	DefaultVocabularyPath nvarchar(1000) NOT NULL,
	Sleep int DEFAULT 10 NOT NULL,
	Beep bit DEFAULT 0 NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	CONSTRAINT PkProfiles PRIMARY KEY (ProfileId)
);

CREATE TABLE Actions
(
	ActionId nvarchar(1) NOT NULL,
	Action nvarchar(100) NOT NULL,
	Weight int NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	CONSTRAINT PkActions PRIMARY KEY (ActionId)
);

CREATE TABLE History
(
	HistoryId uniqueidentifier ROWGUIDCOL NOT NULL,
	VocabularyId uniqueidentifier NOT NULL,
	MeaningId uniqueidentifier NOT NULL,
	ActionId nvarchar(1) NOT NULL,
	SyncMod datetime DEFAULT GETDATE() NOT NULL,
	CONSTRAINT FkHistoryActions FOREIGN KEY (ActionId) REFERENCES Actions (ActionId)
);

CREATE INDEX Idx_History_VocabularyId ON History (VocabularyId);

CREATE INDEX Idx_History_MeaningId ON History (MeaningId);

CREATE INDEX Idx_History_ActionId ON History (ActionId);