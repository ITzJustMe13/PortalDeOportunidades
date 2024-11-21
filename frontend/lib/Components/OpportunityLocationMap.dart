import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';
import 'package:geocode/geocode.dart';

class OpportunityLocationMap extends StatefulWidget {
  final String address;

  const OpportunityLocationMap({Key? key, required this.address}) : super(key: key);

  @override
  _OpportunityLocationMapState createState() => _OpportunityLocationMapState();
}

class _OpportunityLocationMapState extends State<OpportunityLocationMap> {
  double? latitude;
  double? longitude;
  String error = '';

  @override
  void initState() {
    super.initState();
    _getCoordinates();
  }

  // Method to get coordinates from the address
  Future<void> _getCoordinates() async {
    GeoCode geoCode = GeoCode(apiKey: '71779414495299352043x43019');

    try {
      // Perform geocoding to get coordinates based on the address
      Coordinates coordinates = await geoCode.forwardGeocoding(address: widget.address);

      setState(() {
        latitude = coordinates.latitude;
        longitude = coordinates.longitude;
        error = ''; // Clear any error if successful
      });
    } catch (e) {
      setState(() {
        error = 'Error: $e';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    // Default coordinates in case geocoding fails
    double initialLatitude = latitude ?? 0.0;
    double initialLongitude = longitude ?? 0.0;

    return Container(
      height: 250,
      child: latitude == null || longitude == null
          ? Center(child: CircularProgressIndicator()) // Show loading until coordinates are fetched
          : FlutterMap(
              options: MapOptions(
                initialCenter: LatLng(initialLatitude, initialLongitude),
                initialZoom: 16.0,
              ),
              children: [
                TileLayer(
                  urlTemplate: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                  subdomains: ['a', 'b', 'c'],
                ),
                MarkerLayer(
                  markers: [
                    Marker(
                      point: LatLng(initialLatitude, initialLongitude),
                      child: Icon(Icons.pin_drop, color: Colors.red),
                    ),
                  ],
                ),
              ],
            ),
    );
  }
}
