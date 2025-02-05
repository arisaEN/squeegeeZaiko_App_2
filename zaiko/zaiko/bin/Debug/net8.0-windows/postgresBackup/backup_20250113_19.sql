PGDMP  
    5                 }            SqueegeeManagement    17.2    17.2 5    m           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                           false            n           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                           false            o           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                           false            p           1262    32769    SqueegeeManagement    DATABASE     �   CREATE DATABASE "SqueegeeManagement" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'Japanese_Japan.932';
 $   DROP DATABASE "SqueegeeManagement";
                     arisa    false            �            1255    32773    update_timestamp()    FUNCTION     �   CREATE FUNCTION public.update_timestamp() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.更新日 = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;
 )   DROP FUNCTION public.update_timestamp();
       public               postgres    false            �            1259    32774    Mスキージ    TABLE     �  CREATE TABLE public."Mスキージ" (
    "スキージid" integer NOT NULL,
    "名称" character varying(100) NOT NULL,
    "全長" numeric(10,2) NOT NULL,
    "廃棄ライン" numeric(10,2) NOT NULL,
    "登録日" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "更新日" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "削除フラグ" integer DEFAULT 0,
    "スキージ研磨減少量" numeric(10,2) DEFAULT 0.00
);
 #   DROP TABLE public."Mスキージ";
       public         heap r       postgres    false            �            1259    32781    Mスキージ_id_seq    SEQUENCE     �   CREATE SEQUENCE public."Mスキージ_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 -   DROP SEQUENCE public."Mスキージ_id_seq";
       public               postgres    false    217            q           0    0    Mスキージ_id_seq    SEQUENCE OWNED BY     _   ALTER SEQUENCE public."Mスキージ_id_seq" OWNED BY public."Mスキージ"."スキージid";
          public               postgres    false    218            �            1259    32782    M削除    TABLE     �   CREATE TABLE public."M削除" (
    "削除id" integer NOT NULL,
    "名称" character varying(100) NOT NULL,
    "登録日" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "更新日" timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);
    DROP TABLE public."M削除";
       public         heap r       arisa    false            �            1259    32787    M削除_id_seq    SEQUENCE     �   CREATE SEQUENCE public."M削除_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 '   DROP SEQUENCE public."M削除_id_seq";
       public               arisa    false    219            r           0    0    M削除_id_seq    SEQUENCE OWNED BY     M   ALTER SEQUENCE public."M削除_id_seq" OWNED BY public."M削除"."削除id";
          public               arisa    false    220            �            1259    32788    M在庫登録最新    TABLE     E  CREATE TABLE public."M在庫登録最新" (
    "在庫登録状況id" integer NOT NULL,
    "名称" character varying(10) NOT NULL,
    "登録日時" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "更新日時" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "削除フラグ" integer DEFAULT 0
);
 )   DROP TABLE public."M在庫登録最新";
       public         heap r       postgres    false            �            1259    32794 ,   M在庫登録最新_在庫登録状況id_seq    SEQUENCE     �   CREATE SEQUENCE public."M在庫登録最新_在庫登録状況id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 E   DROP SEQUENCE public."M在庫登録最新_在庫登録状況id_seq";
       public               postgres    false    221            s           0    0 ,   M在庫登録最新_在庫登録状況id_seq    SEQUENCE OWNED BY     �   ALTER SEQUENCE public."M在庫登録最新_在庫登録状況id_seq" OWNED BY public."M在庫登録最新"."在庫登録状況id";
          public               postgres    false    222            �            1259    32795    M在庫登録状況    TABLE     E  CREATE TABLE public."M在庫登録状況" (
    "在庫登録状況id" integer NOT NULL,
    "名称" character varying(10) NOT NULL,
    "登録日時" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "更新日時" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "削除フラグ" integer DEFAULT 0
);
 )   DROP TABLE public."M在庫登録状況";
       public         heap r       postgres    false            �            1259    32801 ,   M在庫登録状況_在庫登録状況id_seq    SEQUENCE     �   CREATE SEQUENCE public."M在庫登録状況_在庫登録状況id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 E   DROP SEQUENCE public."M在庫登録状況_在庫登録状況id_seq";
       public               postgres    false    223            t           0    0 ,   M在庫登録状況_在庫登録状況id_seq    SEQUENCE OWNED BY     �   ALTER SEQUENCE public."M在庫登録状況_在庫登録状況id_seq" OWNED BY public."M在庫登録状況"."在庫登録状況id";
          public               postgres    false    224            �            1259    32802    T在庫調査    TABLE       CREATE TABLE public."T在庫調査" (
    "在庫登録id" integer NOT NULL,
    "スキージid" integer NOT NULL,
    "長さ" numeric(10,2) NOT NULL,
    "登録日" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "更新日" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "削除フラグ" integer DEFAULT 1,
    "在庫登録状況フラグ" integer DEFAULT 1,
    "在庫登録最新フラグ" integer DEFAULT 1,
    "フォーム起動日" date,
    "グループ" integer DEFAULT 0 NOT NULL
);
 #   DROP TABLE public."T在庫調査";
       public         heap r       postgres    false            �            1259    32811     T在庫調査_在庫登録id_seq    SEQUENCE     �   CREATE SEQUENCE public."T在庫調査_在庫登録id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 9   DROP SEQUENCE public."T在庫調査_在庫登録id_seq";
       public               postgres    false    225            u           0    0     T在庫調査_在庫登録id_seq    SEQUENCE OWNED BY     k   ALTER SEQUENCE public."T在庫調査_在庫登録id_seq" OWNED BY public."T在庫調査"."在庫登録id";
          public               postgres    false    226            �            1259    32812    T製品投入数    TABLE     U  CREATE TABLE public."T製品投入数" (
    "製品投入id" integer NOT NULL,
    "投入数" integer NOT NULL,
    "登録日時" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "更新日時" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "削除フラグ" integer DEFAULT 0,
    "投入年月日" character(6)
);
 &   DROP TABLE public."T製品投入数";
       public         heap r       postgres    false            �            1259    32818 #   T製品投入数_製品投入id_seq    SEQUENCE     �   CREATE SEQUENCE public."T製品投入数_製品投入id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 <   DROP SEQUENCE public."T製品投入数_製品投入id_seq";
       public               postgres    false    227            v           0    0 #   T製品投入数_製品投入id_seq    SEQUENCE OWNED BY     q   ALTER SEQUENCE public."T製品投入数_製品投入id_seq" OWNED BY public."T製品投入数"."製品投入id";
          public               postgres    false    228            �           2604    32819    Mスキージ スキージid    DEFAULT     �   ALTER TABLE ONLY public."Mスキージ" ALTER COLUMN "スキージid" SET DEFAULT nextval('public."Mスキージ_id_seq"'::regclass);
 O   ALTER TABLE public."Mスキージ" ALTER COLUMN "スキージid" DROP DEFAULT;
       public               postgres    false    218    217            �           2604    32820    M削除 削除id    DEFAULT     t   ALTER TABLE ONLY public."M削除" ALTER COLUMN "削除id" SET DEFAULT nextval('public."M削除_id_seq"'::regclass);
 C   ALTER TABLE public."M削除" ALTER COLUMN "削除id" DROP DEFAULT;
       public               arisa    false    220    219            �           2604    32821 (   M在庫登録最新 在庫登録状況id    DEFAULT     �   ALTER TABLE ONLY public."M在庫登録最新" ALTER COLUMN "在庫登録状況id" SET DEFAULT nextval('public."M在庫登録最新_在庫登録状況id_seq"'::regclass);
 [   ALTER TABLE public."M在庫登録最新" ALTER COLUMN "在庫登録状況id" DROP DEFAULT;
       public               postgres    false    222    221            �           2604    32822 (   M在庫登録状況 在庫登録状況id    DEFAULT     �   ALTER TABLE ONLY public."M在庫登録状況" ALTER COLUMN "在庫登録状況id" SET DEFAULT nextval('public."M在庫登録状況_在庫登録状況id_seq"'::regclass);
 [   ALTER TABLE public."M在庫登録状況" ALTER COLUMN "在庫登録状況id" DROP DEFAULT;
       public               postgres    false    224    223            �           2604    32823    T在庫調査 在庫登録id    DEFAULT     �   ALTER TABLE ONLY public."T在庫調査" ALTER COLUMN "在庫登録id" SET DEFAULT nextval('public."T在庫調査_在庫登録id_seq"'::regclass);
 O   ALTER TABLE public."T在庫調査" ALTER COLUMN "在庫登録id" DROP DEFAULT;
       public               postgres    false    226    225            �           2604    32824    T製品投入数 製品投入id    DEFAULT     �   ALTER TABLE ONLY public."T製品投入数" ALTER COLUMN "製品投入id" SET DEFAULT nextval('public."T製品投入数_製品投入id_seq"'::regclass);
 R   ALTER TABLE public."T製品投入数" ALTER COLUMN "製品投入id" DROP DEFAULT;
       public               postgres    false    228    227            _          0    32774    Mスキージ 
   TABLE DATA           �   COPY public."Mスキージ" ("スキージid", "名称", "全長", "廃棄ライン", "登録日", "更新日", "削除フラグ", "スキージ研磨減少量") FROM stdin;
    public               postgres    false    217   I       a          0    32782    M削除 
   TABLE DATA           S   COPY public."M削除" ("削除id", "名称", "登録日", "更新日") FROM stdin;
    public               arisa    false    219   �I       c          0    32788    M在庫登録最新 
   TABLE DATA           �   COPY public."M在庫登録最新" ("在庫登録状況id", "名称", "登録日時", "更新日時", "削除フラグ") FROM stdin;
    public               postgres    false    221   4J       e          0    32795    M在庫登録状況 
   TABLE DATA           �   COPY public."M在庫登録状況" ("在庫登録状況id", "名称", "登録日時", "更新日時", "削除フラグ") FROM stdin;
    public               postgres    false    223   �J       g          0    32802    T在庫調査 
   TABLE DATA           �   COPY public."T在庫調査" ("在庫登録id", "スキージid", "長さ", "登録日", "更新日", "削除フラグ", "在庫登録状況フラグ", "在庫登録最新フラグ", "フォーム起動日", "グループ") FROM stdin;
    public               postgres    false    225   �J       i          0    32812    T製品投入数 
   TABLE DATA           �   COPY public."T製品投入数" ("製品投入id", "投入数", "登録日時", "更新日時", "削除フラグ", "投入年月日") FROM stdin;
    public               postgres    false    227   K       w           0    0    Mスキージ_id_seq    SEQUENCE SET     D   SELECT pg_catalog.setval('public."Mスキージ_id_seq"', 5, true);
          public               postgres    false    218            x           0    0    M削除_id_seq    SEQUENCE SET     >   SELECT pg_catalog.setval('public."M削除_id_seq"', 2, true);
          public               arisa    false    220            y           0    0 ,   M在庫登録最新_在庫登録状況id_seq    SEQUENCE SET     \   SELECT pg_catalog.setval('public."M在庫登録最新_在庫登録状況id_seq"', 2, true);
          public               postgres    false    222            z           0    0 ,   M在庫登録状況_在庫登録状況id_seq    SEQUENCE SET     \   SELECT pg_catalog.setval('public."M在庫登録状況_在庫登録状況id_seq"', 2, true);
          public               postgres    false    224            {           0    0     T在庫調査_在庫登録id_seq    SEQUENCE SET     Q   SELECT pg_catalog.setval('public."T在庫調査_在庫登録id_seq"', 65, true);
          public               postgres    false    226            |           0    0 #   T製品投入数_製品投入id_seq    SEQUENCE SET     S   SELECT pg_catalog.setval('public."T製品投入数_製品投入id_seq"', 2, true);
          public               postgres    false    228            �           2606    32826     Mスキージ Mスキージ_pkey 
   CONSTRAINT     p   ALTER TABLE ONLY public."Mスキージ"
    ADD CONSTRAINT "Mスキージ_pkey" PRIMARY KEY ("スキージid");
 N   ALTER TABLE ONLY public."Mスキージ" DROP CONSTRAINT "Mスキージ_pkey";
       public                 postgres    false    217            �           2606    32828    M削除 M削除_pkey 
   CONSTRAINT     ^   ALTER TABLE ONLY public."M削除"
    ADD CONSTRAINT "M削除_pkey" PRIMARY KEY ("削除id");
 B   ALTER TABLE ONLY public."M削除" DROP CONSTRAINT "M削除_pkey";
       public                 arisa    false    219            �           2606    32830 ,   M在庫登録最新 M在庫登録最新_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public."M在庫登録最新"
    ADD CONSTRAINT "M在庫登録最新_pkey" PRIMARY KEY ("在庫登録状況id");
 Z   ALTER TABLE ONLY public."M在庫登録最新" DROP CONSTRAINT "M在庫登録最新_pkey";
       public                 postgres    false    221            �           2606    32832 ,   M在庫登録状況 M在庫登録状況_pkey 
   CONSTRAINT     �   ALTER TABLE ONLY public."M在庫登録状況"
    ADD CONSTRAINT "M在庫登録状況_pkey" PRIMARY KEY ("在庫登録状況id");
 Z   ALTER TABLE ONLY public."M在庫登録状況" DROP CONSTRAINT "M在庫登録状況_pkey";
       public                 postgres    false    223            �           2606    32834     T在庫調査 T在庫調査_pkey 
   CONSTRAINT     p   ALTER TABLE ONLY public."T在庫調査"
    ADD CONSTRAINT "T在庫調査_pkey" PRIMARY KEY ("在庫登録id");
 N   ALTER TABLE ONLY public."T在庫調査" DROP CONSTRAINT "T在庫調査_pkey";
       public                 postgres    false    225            �           2606    32836 &   T製品投入数 T製品投入数_pkey 
   CONSTRAINT     v   ALTER TABLE ONLY public."T製品投入数"
    ADD CONSTRAINT "T製品投入数_pkey" PRIMARY KEY ("製品投入id");
 T   ALTER TABLE ONLY public."T製品投入数" DROP CONSTRAINT "T製品投入数_pkey";
       public                 postgres    false    227            �           2606    32838 '   T製品投入数 unique_投入年月日 
   CONSTRAINT     s   ALTER TABLE ONLY public."T製品投入数"
    ADD CONSTRAINT "unique_投入年月日" UNIQUE ("投入年月日");
 U   ALTER TABLE ONLY public."T製品投入数" DROP CONSTRAINT "unique_投入年月日";
       public                 postgres    false    227            �           2620    32839 "   Mスキージ set_update_timestamp    TRIGGER     �   CREATE TRIGGER set_update_timestamp BEFORE UPDATE ON public."Mスキージ" FOR EACH ROW EXECUTE FUNCTION public.update_timestamp();
 =   DROP TRIGGER set_update_timestamp ON public."Mスキージ";
       public               postgres    false    229    217            �           2620    32840    M削除 set_update_timestamp    TRIGGER        CREATE TRIGGER set_update_timestamp BEFORE UPDATE ON public."M削除" FOR EACH ROW EXECUTE FUNCTION public.update_timestamp();
 7   DROP TRIGGER set_update_timestamp ON public."M削除";
       public               arisa    false    229    219            �           2620    32841 (   M在庫登録最新 set_update_timestamp    TRIGGER     �   CREATE TRIGGER set_update_timestamp BEFORE UPDATE ON public."M在庫登録最新" FOR EACH ROW EXECUTE FUNCTION public.update_timestamp();
 C   DROP TRIGGER set_update_timestamp ON public."M在庫登録最新";
       public               postgres    false    229    221            �           2620    32842 (   M在庫登録状況 set_update_timestamp    TRIGGER     �   CREATE TRIGGER set_update_timestamp BEFORE UPDATE ON public."M在庫登録状況" FOR EACH ROW EXECUTE FUNCTION public.update_timestamp();
 C   DROP TRIGGER set_update_timestamp ON public."M在庫登録状況";
       public               postgres    false    229    223            �           2620    32843 "   T在庫調査 set_update_timestamp    TRIGGER     �   CREATE TRIGGER set_update_timestamp BEFORE UPDATE ON public."T在庫調査" FOR EACH ROW EXECUTE FUNCTION public.update_timestamp();
 =   DROP TRIGGER set_update_timestamp ON public."T在庫調査";
       public               postgres    false    225    229            _   �   x�}н�0��<���ϱ]"�`&��c*�-�E$"E��d=�OwVz?/3(8@���'�I�A�E4�SAL�	� ��5��^��@��I�}�V��ìy��9׺�p(�wlHF�c�2��o]���p��t/�"9��:.�����Mf���m��WH1�n�+`	h�L����)�+�O)      a   W   x���A�0��b����F�k��p@� �_ҳ��v\����"(�,>8�Y�ɣ�G^X
G�谨�p�����΢��JD/� �      c   L   x�3�|޿���FF����
��V&VF�zF��������8_6�=�ۍ���X��������W� ��      e   R   x�3�|�`���+����4202�50�5�P04�20�26�302442�'e�e���u���X�0�21�317�40�'e����� �7"�      g      x������ � �      i   D   x�}��� Dkn
 �΀0���DQʈ�}�hI�V����˹4����]7B��#�ټ��!�
� )�}     