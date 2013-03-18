/* Create Version Table */
CREATE TABLE "main"."_version"(
  "dbver" INT NOT NULL DEFAULT 0,
  "dbdate" INT NOT NULL DEFAULT 0,
  PRIMARY KEY ("dbver")
);

/* Insert Default Version */
INSERT INTO "main"."_version" VALUES(2, 0);

/* Add lastip Column */
ALTER TABLE "main"."accounts" ADD COLUMN "lastip" TEXT DEFAULT NULL;