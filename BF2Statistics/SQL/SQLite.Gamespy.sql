CREATE TABLE "main"."accounts" (
	"id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	"name"  TEXT NOT NULL UNIQUE COLLATE NOCASE,
	"password"  TEXT NOT NULL,
	"email"  TEXT NOT NULL,
	"country"  TEXT NOT NULL,
	"lastip" TEXT DEFAULT NULL,
	"session"  INTEGER DEFAULT 0
);


CREATE TABLE "main"."_version"(
  "dbver" INT NOT NULL DEFAULT 0,
  "dbdate" INT NOT NULL DEFAULT 0,
  PRIMARY KEY ("dbver")
);

INSERT INTO "main"."_version" VALUES(2, 0);