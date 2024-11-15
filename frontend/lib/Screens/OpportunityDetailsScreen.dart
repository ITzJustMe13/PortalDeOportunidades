import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:frontend/Screens/CustomAppBar.dart';
import 'package:frontend/Screens/CustomDrawer.dart';
import 'package:frontend/Enums/OppLocation.dart'; // Renamed enum to avoid conflict
import 'package:frontend/Enums/OppCategory.dart';
import 'package:geocoding/geocoding.dart';
import 'package:latlong2/latlong.dart'; // For LatLng

class OpportunityDetailsScreen extends StatefulWidget {
  @override
  _OpportunityDetailsScreenState createState() =>
      _OpportunityDetailsScreenState();
}

class _OpportunityDetailsScreenState extends State<OpportunityDetailsScreen> {
  final String _name = 'Venha Aprender a Fazer Posta à Transmontana';
  final double _price = 14.99;
  final int _vacancies = 2;
  final OppCategory _oppcategory = OppCategory.COZINHA_TIPICA;
  final String _description =
      'Venha aprender a arte tradicional de Trás-os-Montes num dos mais antigos restaurantes do país.';
  final OppLocation _location = OppLocation.VILA_REAL; // Renamed to AppLocation
  final String _address = 'R. Padre Filipe Borges 6 Vila Real';
  final String _firstName = 'António';
  final String _lastName = 'Sousa';
  final String _time = '10:00/11:00';
  final DateTime _date = DateTime.now();

  

  // List of image paths
  final List<String> _imagePaths = [
    'assets/images/opp.jpg',
    'assets/images/opp.jpg',
    'assets/images/opp.jpg',
  ];

  // Page controller for PageView
  final PageController _pageController = PageController();
  int _currentPage = 0;
String _addressTest = "Loading..."; 

  @override
  void initState() {
    super.initState();
    //_fetchAddress();
  }

  List<double>? _coordinates = [41.3062303, -7.7475837];
  

/*
Future<void> _fetchAddress() async {
    final double lat = 37.7749;  // Example latitude (San Francisco)
    final double lon = -122.4194; // Example longitude (San Francisco)

    // Call the async function and wait for the result
    String? address = await getAddressFromCoordinates(lat, lon);

    // Update the state with the fetched address
    setState(() {
      _addressTest = address ?? "Error fetching address";  // Set default if null
    });
  }
  
Future<String?> getAddressFromCoordinates(double latitude, double longitude) async {
  try {
    // Perform reverse geocoding
    List<Placemark> placemarks = await placemarkFromCoordinates(latitude, longitude);

    // If the result is not empty, return the address
    if (placemarks.isNotEmpty) {
      Placemark place = placemarks.first;
      return "Address: ${place.street}, ${place.locality}, ${place.country}";
    } else {
      return "No address found for the provided coordinates.";
    }
  } catch (e) {
    print("Error fetching address: $e");
    return null;  // Return null in case of an error
  }
}
*/

  void _nextPage() {
    if (_currentPage < _imagePaths.length - 1) {
      setState(() {
        _currentPage++;
        _pageController.animateToPage(
          _currentPage,
          duration: Duration(milliseconds: 300),
          curve: Curves.easeInOut,
        );
      });
    }
  }

  void _previousPage() {
    if (_currentPage > 0) {
      setState(() {
        _currentPage--;
        _pageController.animateToPage(
          _currentPage,
          duration: Duration(milliseconds: 300),
          curve: Curves.easeInOut,
        );
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(20.0),
          child: Column(
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  // Left Column (Image + Description)
                  Expanded(
                    flex: 2,
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 40.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Text(
                          _name,
                          style: const TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                            color: Colors.black,
                          ),
                        ),
                        const SizedBox(height: 16),
                        // Display multiple images using PageView with navigation buttons
                        Stack(
                          children: [
                            SizedBox(
                              height: 500,
                              width: 700,
                              child: PageView.builder(
                                controller: _pageController,
                                itemCount: _imagePaths.length,
                                onPageChanged: (int index) {
                                  setState(() {
                                    _currentPage = index;
                                  });
                                },
                                itemBuilder: (context, index) {
                                  return ClipRRect(
                                    borderRadius: BorderRadius.circular(10.0),
                                    child: Image.asset(
                                      _imagePaths[index],
                                      fit: BoxFit.cover,
                                    ),
                                  );
                                },
                              ),
                            ),
                            // Left button (Centered on the start)
                            Positioned(
                              left: 10,
                              top: 200,
                              child: IconButton(
                                icon: Icon(Icons.arrow_back_ios, color: Colors.white),
                                onPressed: _previousPage,
                              ),
                            ),
                            // Right button (Centered on the end)
                            Positioned(
                              right: 10,
                              top: 200,
                              child: IconButton(
                                icon: Icon(Icons.arrow_forward_ios, color: Colors.white),
                                onPressed: _nextPage,
                              ),
                            ),
                            // Dot indicators
                            Positioned(
                              bottom: 10,
                              left: 0,
                              right: 0,
                              child: Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: List.generate(
                                  _imagePaths.length,
                                  (index) => AnimatedContainer(
                                    duration: const Duration(milliseconds: 300),
                                    margin: const EdgeInsets.symmetric(horizontal: 4),
                                    width: _currentPage == index ? 12 : 8,
                                    height: 8,
                                    decoration: BoxDecoration(
                                      color: _currentPage == index
                                          ? Colors.green
                                          : Colors.grey,
                                      shape: BoxShape.circle,
                                    ),
                                  ),
                                ),
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 8),
                        Text(
                          'Descrição:',
                          style: const TextStyle(
                            fontSize: 25,
                            fontWeight: FontWeight.w600,
                            color: Colors.green,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Container(
                          width: 500.0, // Set the maximum width here
                          child: Text(
                            _description,
                            style: const TextStyle(
                              fontSize: 20,
                              color: Colors.black87,
                            ),
                            overflow: TextOverflow.visible,
                          ),
                        ),
                        const SizedBox(height: 20),
                        Row(children: <Widget>[
                          Text(
                            'Opportunidade Criada por $_firstName $_lastName',
                            style: const TextStyle(
                              fontSize: 16,
                              color: Colors.black87,
                            ),
                          ),
                          const SizedBox(width: 200),
                          Text(
                            'Horário: $_time',
                            style: const TextStyle(
                              fontSize: 16,
                              color: Colors.black87,
                            ),
                          )
                        ]),
                      ],
                    ),
                  ),
                ),
                  // Right Column (Location, Price, etc.)
                  Expanded(
                    flex: 1,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const SizedBox(height: 70),
                        Text(
                          'Localização: ${_location.name}',
                          style: const TextStyle(fontSize: 16, color: Colors.black54),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          'Endereço: $_address',
                          style: const TextStyle(fontSize: 16, color: Colors.black54),
                        ),
                        const SizedBox(height: 16),
                        Row(
                          crossAxisAlignment: CrossAxisAlignment.center,
                          children: <Widget>[
                            Text(
                              'Preço: $_price € / Pessoa',
                              style: const TextStyle(fontSize: 18, color: Colors.green),
                            ),
                            Text(
                              '$_vacancies Vagas Disponíveis',
                              style: const TextStyle(fontSize: 18, color: Colors.green),
                            ),
                          ],
                        ),
                        const SizedBox(height: 16),
                        // FlutterMap Widget for showing the map
                        Container(
                          height: 300, // Height of the map
                          child: _coordinates!= null
                              ? FlutterMap(
                                  options: MapOptions(
                                    initialCenter: LatLng(_coordinates?[0]?? 0.0, _coordinates?[1]?? 0.0),
                                    initialZoom: 16.0, // Adjust zoom level
                                  ),
                                  children: [
                                    // Tile Layer for the map tiles
                                    TileLayer(
                                      urlTemplate: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                                      subdomains: ['a', 'b', 'c'],
                                    ),
                                    // Marker Layer for showing markers
                                    MarkerLayer(
                                      markers: [
                                        Marker(
                                          width: 80.0,
                                          height: 80.0,
                                          point: LatLng(_coordinates?[0]?? 0.0, _coordinates?[1]?? 0.0),
                                          child: Icon(Icons.pin_drop, color: Colors.red),
                                        ),
                                      ],
                                    ),
                                  ],
                                )
                              : Center(child: CircularProgressIndicator()),
                        ),
                        const SizedBox(height: 16),
                        Text(
                          'Latitude: ${_coordinates?[0]?? 0.0}° ${(_coordinates?[0]?? 0.0).toStringAsFixed(6)}″N, Longitude: ${_coordinates?[1]?? 0.0}° ${(_coordinates?[1]?? 0.0).toStringAsFixed(6)}″E',
                          style: TextStyle(fontSize: 16),
                        ),
                        const SizedBox(height: 16),
                        Text(
                          _addressTest,
                          style: TextStyle(fontSize: 16),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}

