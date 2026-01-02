-- TestProjects table for initial project setup
-- This table is used for testing and learning database interactions

CREATE TABLE IF NOT EXISTS "TestProjects" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL
);

-- Insert mock data
INSERT INTO "TestProjects" ("Name") VALUES
    ('Sample Project 1'),
    ('Sample Project 2'),
    ('Sample Project 3'),
    ('Learning Project'),
    ('Test Project');
