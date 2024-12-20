enum OppCategory
{
    AGRICULTURA,
    ARTESANATO,
    COZINHA_TIPICA,
    PASTELARIA,
    ARTES_CIENCIAS,
    NATUREZA_E_AVENTURA,
    DESPORTOS_ATIVIDADES_AO_AR_LIVRE,
    AULAS_E_CURSOS
}

// Convert an integer (from the backend) to the corresponding Category enum
OppCategory categoryFromInt(int categoryInt) {
  return OppCategory.values[categoryInt]; // This works because Flutter's enums are indexed
}

// Convert a Category enum to its integer value for sending to the backend
int categoryToInt(OppCategory category) {
  return category.index; // This gives the integer value of the enum
}
