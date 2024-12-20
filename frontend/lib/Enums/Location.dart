enum Location
{
    VIANA_DO_CASTELO,
    BRAGA,
    VILA_REAL,
    BRAGANCA,
    PORTO,
    AVEIRO,
    VISEU,
    GUARDA,
    COIMBRA,
    CASTELO_BRANCO,
    LEIRIA,
    LISBOA,
    SANTAREM,
    EVORA,
    PORTALEGRE,
    SETUBAL,
    BEJA,
    FARO,
    MADEIRA,
    ACORES
}

// Convert an integer (from the backend) to the corresponding Location enum
Location locationFromInt(int locationInt) {
  return Location.values[locationInt]; // This works because Flutter's enums are indexed
}

// Convert a Location enum to its integer value for sending to the backend
int locationToInt(Location location) {
  return location.index; // This gives the integer value of the enum
}
