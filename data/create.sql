USE master;
DROP DATABASE TriviapiDB;
GO
CREATE DATABASE TriviapiDB;
GO
USE TriviapiDB;
GO


--------------------------------------------------------------
--  Table Creation
--------------------------------------------------------------

CREATE TABLE Tag (
    TagID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TagName VARCHAR(30) UNIQUE NOT NULL
);
GO

CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryName VARCHAR(30) UNIQUE NOT NULL
);
GO


CREATE TABLE Difficulty (
    DifficultyID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    DifficultyName VARCHAR(30) UNIQUE NOT NULL
);
GO

CREATE TABLE [Status] (
    StatusID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    StatusName VARCHAR(30) UNIQUE NOT NULL
);
GO


CREATE TABLE Question (
    QuestionID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Question VARCHAR(255) UNIQUE NOT NULL,
	Answer VARCHAR(255) NOT NULL,
	CategoryID INT NOT NULL,
	DifficultyID INT NOT NULL,
	StatusID INT NOT NULL DEFAULT 1,


	CONSTRAINT [FK_Question.CategoryID] FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
	CONSTRAINT [FK_Question.DifficultyID] FOREIGN KEY (DifficultyID) REFERENCES Difficulty(DifficultyID),
	CONSTRAINT [FK_Question.StatusID] FOREIGN KEY (StatusID) REFERENCES [Status](StatusID),
);
GO

CREATE TABLE QuestionTag (
     QuestionTagID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
     TagID INT NOT NULL,
	 QuestionID INT NOT NULL,
	 CONSTRAINT [FK_QuestionTag.TagID] FOREIGN KEY (TagID) REFERENCES Tag(TagID),
	 CONSTRAINT [FK_QuestionTag.QuestionID] FOREIGN KEY (QuestionID) REFERENCES Question(QuestionID)
);
GO
