# match_data migrations

CREATE TABLE match_data.football_match_data (
	id BIGINT NOT NULL AUTO_INCREMENT,
	division VARCHAR(30) NOT NULL,
	match_date DATE NOT NULL,
	home_team VARCHAR(30) NOT NULL,
	away_team VARCHAR(30) NOT NULL,
	ft_hg TINYINT NOT NULL,
	ft_ag TINYINT NOT NULL,
	ft_r VARCHAR(1) NOT NULL,
	ht_hg TINYINT NULL,
	ht_ag TINYINT NULL,
	ht_r VARCHAR(1) NULL,
	season VARCHAR(30) NOT NULL,
	created_on TIMESTAMP NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
	updated_on TIMESTAMP NULL,
 	PRIMARY KEY (id),
 	CONSTRAINT uc_match_date_home_team UNIQUE(match_date, home_team)
)
DEFAULT CHARSET=utf8
COLLATE=utf8_general_ci;

# load data from S3

LOAD DATA FROM S3 's3://ed-match-data-dropper-stack-matchdatabucket-1egcf9v3ca03e/E0.csv'
INTO TABLE match_data.football_match_data
FIELDS TERMINATED BY ','
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(division,
@match_date,
@ignore_time,
home_team,
away_team,
ft_hg,
ft_ag,
ft_r,
ht_hg,
ht_ag,
ht_r)
SET match_date = STR_TO_DATE(@match_date, '%d/%m/%Y');