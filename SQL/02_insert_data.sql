--
-- PostgreSQL database dump
--

\restrict 9PZCU648SrqWAdQj2IcZLLUEwopbxRtLSMTvjXHf70zC0Lx3K1ZdDde55tFK3TT

-- Dumped from database version 18.6
-- Dumped by pg_dump version 18.6

-- Started on 2026-11-19 21:01:37

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 5039 (class 0 OID 16465)
-- Dependencies: 222
-- Data for Name: heroes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.heroes (id, name) FROM stdin;
1	Alpha
2	Kagura
3	Esmeralda
4	Karina
5	Vale
6	Layla
7	Miya
8	Eudora
9	Zilong
10	Melissa
11	Lolita
12	Ixia
13	Lylia
14	Freya
15	Estes
16	Bruno
17	Kimmy
18	Wanwan
19	Claude
20	Johnson
\.


--
-- TOC entry 5037 (class 0 OID 16452)
-- Dependencies: 220
-- Data for Name: rarities; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rarities (id, name, tier, color_hex) FROM stdin;
1	UR	Ultra Rare	#FFD700
2	RS	Rare Shiny	#9B59B6
3	QR	Special Card	#FF69B4
4	R	Rare	#3498DB
\.


--
-- TOC entry 5041 (class 0 OID 16476)
-- Dependencies: 224
-- Data for Name: cards; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cards (id, card_code, card_name, price, times_viewed, image_url, rarity_id, hero_id, description) FROM stdin;
1	ML-AS-UR027	The Protector	212.00	456	images/ur027_alpha.png	1	1	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
2	ML-AS-UR026	Starlight	22.00	298	images/ur026_vale.png	1	5	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
3	ML-AS-UR023	Lolita	117.00	368	images/ur023_lolita.png	1	11	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
4	ML-AS-RS004	Flower Season	199.00	424	images/rs004_kagura.png	2	2	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
5	ML-AS-RS020	Esmeralda	0.10	1102	images/rs020_esmeralda.png	2	3	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
6	ML-AS-QR001	Miya	0.15	1483	images/qr001_miya.png	3	7	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
7	ML-AS-QR002	Eudora	0.15	1483	images/qr002_eudora.png	3	8	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
8	ML-AS-R003	Green Flash	0.15	1483	images/r003_layla.png	4	6	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
9	ML-AS-R001	Burning Bow	0.05	1252	images/r001_miya.png	4	7	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
10	ML-AS-R012	Shifting Sand	0.05	995	images/r012_ixia.png	4	12	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
11	ML-AS-R004	Flower Season	0.10	997	images/r004_kagura.png	4	2	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
14	ML-AS-R007	Phantom Blade	0.15	928	images/r007_karina.png	4	4	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
16	ML-AS-R009	Soldier In Training	0.05	573	images/r009_lolita.png	4	11	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
17	ML-AS-R010	Cerulean Winds	0.05	1758	images/r010_vale.png	4	5	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
18	ML-AS-R013	Elite Warrior	0.05	970	images/r013_zilong.png	4	9	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
21	ML-AS-R016	Twinge Weaver	0.05	578	images/r016_melissa.png	4	10	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
12	ML-AS-R005	School Idol	0.05	1380	images/r005_lylia.png	4	13	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
13	ML-AS-R006	Dark Rose	0.05	1645	images/r006_freya.png	4	14	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
15	ML-AS-R008	Darting Star	0.05	1673	images/r008_wanwan.png	4	18	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
19	ML-AS-R014	Holy Priest	0.05	713	images/r014_estes.png	4	15	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
20	ML-AS-R015	Golden Bullet	0.05	575	images/r015_claude.png	4	19	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
25	ML-AS-R020	Ruddy Dusk	0.05	1635	images/r020_esmeralda.png	4	3	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
26	ML-AS-RS001	Miya	0.20	1000	images/rs001_miya.png	2	7	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
27	ML-AS-RS003	Lolita	0.20	1000	images/rs003_lolita.png	2	11	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
30	ML-AS-RS007	Karina	0.20	1000	images/rs007_karina.png	2	4	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
31	ML-AS-RS009	Lolita	0.20	1000	images/rs009_lolita.png	2	11	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
32	ML-AS-RS010	Vale	0.20	1000	images/rs010_vale.png	2	5	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
33	ML-AS-RS011	Alpha	0.20	1000	images/rs011_alpha.png	2	1	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
34	ML-AS-RS013	Zilong	0.20	1000	images/rs013_zilong.png	2	9	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
36	ML-AS-RS016	Melissa	0.20	1000	images/rs016_melissa.png	2	10	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
39	ML-AS-QR003	Layla	0.15	1000	images/qr003_layla.png	3	6	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
40	ML-AS-QR004	Kagura	0.15	1000	images/qr004_kagura.png	3	2	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
28	ML-AS-RS005	Lylia	0.20	1000	images/rs005_lylia.png	2	13	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
29	ML-AS-RS006	Freya	0.20	1000	images/rs006_freya.png	2	14	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
35	ML-AS-RS014	Estes	0.20	1000	images/rs014_estes.png	2	15	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
37	ML-AS-RS017	Bruno	0.20	1000	images/rs017_bruno.png	2	16	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
38	ML-AS-RS018	Kimmy	0.20	1000	images/rs018_kimmy.png	2	17	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
41	ML-AS-UR015	Freya	10.00	1000	images/ur015_freya.png	1	14	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
22	ML-AS-R017	Vanguard Elite	0.05	1376	images/r017_bruno.png	4	16	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
23	ML-AS-R018	Steam Researcher	0.05	1723	images/r018_kimmy.png	4	17	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
24	ML-AS-R019	Fire Chef	0.05	704	images/r019_johnson.png	4	20	This is a legendary card from the MLBB Trading Cards series. High demand among collectors.
\.


--
-- TOC entry 5043 (class 0 OID 16501)
-- Dependencies: 226
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (id, username, password_hash, is_admin) FROM stdin;
1	admin	12345	t
\.


--
-- TOC entry 5049 (class 0 OID 0)
-- Dependencies: 223
-- Name: cards_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cards_id_seq', 47, true);


--
-- TOC entry 5050 (class 0 OID 0)
-- Dependencies: 221
-- Name: heroes_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.heroes_id_seq', 20, true);


--
-- TOC entry 5051 (class 0 OID 0)
-- Dependencies: 219
-- Name: rarities_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.rarities_id_seq', 4, true);


--
-- TOC entry 5052 (class 0 OID 0)
-- Dependencies: 225
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_id_seq', 1, true);


-- Completed on 2026-11-19 21:01:38

--
-- PostgreSQL database dump complete
--

\unrestrict 9PZCU648SrqWAdQj2IcZLLUEwopbxRtLSMTvjXHf70zC0Lx3K1ZdDde55tFK3TT

