-- USERS
INSERT INTO USERS (NAME, USERNAME, EMAIL, PASSWORD)
SELECT 
    'User ' || i,
    'username' || i,
    'user' || i || '@example.com',
    'hashed_password_' || i
FROM generate_series(1, 10) AS s(i);

-- Get 10 user IDs to use for foreign keys
WITH user_ids AS (
    SELECT ID FROM USERS LIMIT 10
)
-- ACCOUNTS IS SEEDED VIA TRIGGER

-- ATLAS
INSERT INTO ATLAS (TITLE)
SELECT 'Atlas #' || i
FROM generate_series(1, 10) AS s(i);

-- Get 10 atlas IDs for linking to owners and markers
WITH atlas_ids AS (
    SELECT ATLAS_ID FROM ATLAS LIMIT 10
),
user_ids AS (
    SELECT ID FROM USERS LIMIT 10
)
-- OWNERS (each user owns one atlas for simplicity)
INSERT INTO OWNERS (OWNER_ID, ATLAS_ID)
SELECT 
    u.ID,
    a.ATLAS_ID
FROM user_ids u
JOIN atlas_ids a ON u.ID IS NOT NULL AND a.ATLAS_ID IS NOT NULL
LIMIT 10;

---- MARKERS (each atlas gets at least one marker)
--WITH atlas_ids AS (
--    SELECT ATLAS_ID FROM ATLAS
--)
--INSERT INTO MARKERS (ATLAS_ID, TITLE, JOURNAL, COORDINATES)
--SELECT 
--    a.ATLAS_ID,
--    'Marker ' || i,
--    'Journal entry for marker ' || i,
--    POINT(random() * 180 - 90, random() * 360 - 180)
--FROM atlas_ids a,
--     generate_series(1, 10) AS s(i)
--LIMIT 10;

-- Get 10 marker IDs for images
WITH atlas_ids AS (
    SELECT ATLAS_ID FROM ATLAS LIMIT 10
)
-- IMAGES
INSERT INTO IMAGES (ATLAS_ID, IMAGE_DETAILS)
SELECT 
    m.ATLAS_ID,
    jsonb_build_array(
        jsonb_build_object(
            'url', 'https://example.com/image' || i || '.jpg',
            'legend', 'Image for marker ' || i,
            'imageId',gen_random_uuid()
        )
    )
FROM atlas_ids m,
     generate_series(1, 10) AS s(i)
LIMIT 10;
