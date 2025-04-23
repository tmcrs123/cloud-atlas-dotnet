CREATE TABLE IF NOT EXISTS USERS (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    NAME VARCHAR(50) NOT NULL, 
    USERNAME VARCHAR(30) NOT NULL, 
    EMAIL VARCHAR(50) NOT NULL, 
    PASSWORD VARCHAR(300) NOT NULL, 
    LAST_UPDATED TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS ACCOUNTS (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    USER_ID UUID NOT NULL, 
    VERIFIED BOOLEAN NOT NULL,  
    LAST_UPDATED TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (USER_ID) REFERENCES USERS(ID) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ATLAS (
	ATLAS_ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID (),
	TITLE VARCHAR(30) NOT NULL,
	LAST_UPDATED TIMESTAMP DEFAULT NOW() NULL
);

CREATE TABLE IF NOT EXISTS OWNERS (
	OWNER_ID UUID,
	ATLAS_ID UUID,
	LAST_UPDATED TIMESTAMP DEFAULT NOW() NULL,
	PRIMARY KEY (OWNER_ID, ATLAS_ID),
	FOREIGN KEY (ATLAS_ID) REFERENCES ATLAS (ATLAS_ID) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS IMAGES (
	ATLAS_ID UUID PRIMARY KEY,
	IMAGE_DETAILS JSONB NULL,
	FOREIGN KEY (ATLAS_ID) REFERENCES ATLAS (ATLAS_ID) ON DELETE CASCADE
);

-- triggers
CREATE OR REPLACE FUNCTION create_user_account()
RETURNS TRIGGER AS $$
BEGIN
    insert into accounts (user_id, verified) values (NEW.id, false);
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER create_user_account_trigger
AFTER INSERT
ON USERS
FOR EACH ROW
EXECUTE FUNCTION create_user_account();

-- procedures

CREATE OR REPLACE PROCEDURE INSERT_IMAGE(
    image_url VARCHAR(200),
    legend VARCHAR(100),
    id_of_atlas UUID,
    OUT affected_rows INT
) 
LANGUAGE plpgsql AS $$
DECLARE
    image_id UUID;
BEGIN
    -- Generate a new UUID for the image
    SELECT gen_random_uuid() INTO image_id;

    -- Update the images table by appending a new image entry to the JSONB array
    UPDATE images
    SET image_details = COALESCE(image_details, '[]'::jsonb) || jsonb_build_array(
        jsonb_build_object(
            'imageId', image_id,
            'legend', legend, 
            'url', image_url
        )
    )
    WHERE images.atlas_id = id_of_atlas;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
END;
$$;

CREATE OR REPLACE PROCEDURE UPDATE_IMAGE(
    image_id TEXT,
    id_of_atlas UUID,
    legend VARCHAR(200),
    OUT affected_rows INT
)
LANGUAGE plpgsql 
AS $$ 
BEGIN
    UPDATE images
    SET image_details = (
        SELECT jsonb_agg(
            CASE 
                WHEN elem->>'imageId' = image_id THEN 
                    jsonb_set(elem, '{legend}', to_jsonb(legend))
                ELSE 
                    elem 
            END
        )
        FROM jsonb_array_elements(image_details) AS elem
    )
    WHERE atlas_id = id_of_atlas;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
END; 
$$;

-- delete an image
CREATE OR REPLACE PROCEDURE DELETE_IMAGE(
    image_id TEXT,
    id_of_atlas UUID,
    OUT affected_rows INT
)
LANGUAGE plpgsql AS $$
BEGIN
	UPDATE images
	SET image_details = (
		select jsonb_agg(elem)
		FROM jsonb_array_elements(image_details) AS elem
		where elem ->> 'imageId' <> image_id
	)
	WHERE atlas_id = id_of_atlas;
    GET DIAGNOSTICS affected_rows = ROW_COUNT;
END
$$