CREATE TABLE Workers (
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	Name varchar (30) NOT NULL,
	WorkerID varchar (30) NOT NULL UNIQUE
);

INSERT INTO Workers (name, WorkerID)
VALUES
('Barak Aharon', 212324252),
('Itay Sarusi', 324260009)