enum Gender
{
    MASCULINO,
    FEMININO,
    OUTRO,
    NAO_ESPECIFICADO
}

// Convert an integer (from the backend) to the corresponding Gender enum
Gender genderFromInt(int genderInt) {
  return Gender.values[genderInt];
}

// Convert a Gender enum to its integer value for sending to the backend
int genderToInt(Gender gender) {
  return gender.index;
}