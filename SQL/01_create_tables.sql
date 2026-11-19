
-- Таблица героев
CREATE TABLE IF NOT EXISTS heroes
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- Таблица редкости карточек
CREATE TABLE IF NOT EXISTS rarities
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(10) NOT NULL UNIQUE,
    tier VARCHAR(50) NOT NULL,
    color_hex VARCHAR(7) NOT NULL
);

-- Таблица карточек
CREATE TABLE IF NOT EXISTS cards
(
    id SERIAL PRIMARY KEY,
    card_code VARCHAR(20) NOT NULL UNIQUE,
    card_name VARCHAR(100) NOT NULL,
    price NUMERIC(10,2) NOT NULL,
    times_viewed INTEGER DEFAULT 0,
    image_url VARCHAR(255) NOT NULL,
    rarity_id INTEGER,
    hero_id INTEGER,
    description TEXT,

    CONSTRAINT cards_hero_id_fkey
        FOREIGN KEY (hero_id)
        REFERENCES heroes(id)
        ON DELETE CASCADE,

    CONSTRAINT cards_rarity_id_fkey
        FOREIGN KEY (rarity_id)
        REFERENCES rarities(id)
        ON DELETE SET NULL
);

-- Таблица пользователей
CREATE TABLE IF NOT EXISTS users
(
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    is_admin BOOLEAN DEFAULT FALSE
);
