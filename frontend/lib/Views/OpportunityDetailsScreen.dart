import 'package:flutter/material.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:frontend/Components/ReviewCard.dart';
import 'package:frontend/Models/Review.dart';
import 'package:latlong2/latlong.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityImages.dart';
import 'package:frontend/Components/OpportunityLocationMap.dart';
import 'package:frontend/Components/OpportunityDetails.dart';
import 'package:frontend/Components/OpportunityAdditionalInfo.dart';
import 'package:frontend/Components/ReservationButton.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Enums/Location.dart';

class OpportunityDetailsScreen extends StatelessWidget {
  final ScrollController verticalScrollController = ScrollController();
  final ScrollController horizontalScrollController = ScrollController();
  //MOCK INFO
  final String _name = 'Venha Aprender a Fazer Posta à Transmontana';
  final String _description = 'Venha aprender a arte tradicional de Trás-os-Montes.\nVenha aprender a arte tradicional de Trás-os-Montes.\nVenha aprender a arte tradicional de Trás-os-Montes.\nVenha aprender a arte tradicional de Trás-os-Montes.';
  final String _address = 'R. Silvestre Vaz 38 Vila Real';
  final double _price = 14.99;
  final int _vacancies = 2;
  final String _firstName = 'António';
  final String _lastName = 'Sousa';
  final String _time = '10:00/11:00';
  final DateTime _date = DateTime.now();
  final Location _location = Location.VILA_REAL;
  final List<Review> _reviews = [
      Review(reservationId: 101, rating: 4.5, description: 'Gostei Bastante!'),
      Review(reservationId: 102, rating: 0.0, description: 'Não gostei e não recomendo.'),
      Review(reservationId: 103, rating: 5.0, description: 'Adorei, incrivel!'),
      Review(reservationId: 103, rating: 2.5, description: 'Foi ok.')
      ];
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            // Layout for small screens (smartphones)
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            // Layout for medium screens (tablets)
            return _buildTabletLayout();
          } else {
            // Layout for large screens (desktops)
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  // Mobile layout (Vertical scroll)
  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          OpportunityImages(),
          SizedBox(height: 20),
          OpportunityDetails(
            name: _name,
            description: _description,),
          SizedBox(height: 20),
          ReservationButton(
            availableVacancies: 2,
            onPressed: () {
              print('Reservation button pressed!');
            },
          ),
          SizedBox(height: 20),
          OpportunityAdditionalInfo(
            price: _price,
            location: _location,
            address: _address,
            vacancies: _vacancies,
            firstName: _firstName,
            lastName: _lastName,
            time: _time,
            date: _date,
          ),
          SizedBox(height: 20),
          OpportunityLocationMap(address: _address,),
          //REVIEWS
          SizedBox(height: 20),
          Text(
          'Opiniões',
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
        SizedBox(height: 10),
        ..._reviews.map((review) => ReviewCard(
            rating: review.rating,
            description: review.description ?? '',
            )).toList(),
        ],
      ),
    );
  }

  // Tablet layout (Vertical scroll with Scrollbar)
    Widget _buildTabletLayout() {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left Column (Image and Details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityImages(),
                  OpportunityDetails(
                    name: _name,
                    description: _description,
                  ),
                  ReservationButton(
                    availableVacancies: 2,
                    onPressed: () {
                      print('Reservation button pressed!');
                    },
                  ),
                ],
              ),
            ),
            SizedBox(width: 20), // Add space between the two columns
            
            // Right Column (Additional Info and Map)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: _price,
                    location: _location,
                    address: _address,
                    vacancies: _vacancies,
                    firstName: _firstName,
                    lastName: _lastName,
                    time: _time,
                    date: _date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(address: _address,),
                  //REVIEWS
                  SizedBox(height: 20),
                  Text(
                  'Opiniões',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  ..._reviews.map((review) => ReviewCard(
                      rating: review.rating,
                      description: review.description ?? '',
                      )).toList(),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

 // Desktop layout (Both vertical and horizontal scroll with Scrollbar)
  Widget _buildDesktopLayout() {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        scrollDirection: Axis.vertical,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left column (images and details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SizedBox(height: 20),
                  OpportunityImages(),
                  OpportunityDetails(
                    name: _name,
                    description: _description,),
                  SizedBox(height: 20),
                  ReservationButton(
                    availableVacancies: 2,
                    onPressed: () {
                      print('Reservation button pressed!');
                    },
                  ),
                ],
              ),
            ),
            SizedBox(width: 20),
            // Right column (additional info and map)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: _price,
                    location: _location,
                    address: _address,
                    vacancies: _vacancies,
                    firstName: _firstName,
                    lastName: _lastName,
                    time: _time,
                    date: _date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(address: _address,),
                  //REVIEWS
                  SizedBox(height: 20),
                  Text(
                  'Opiniões',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  ..._reviews.map((review) => ReviewCard(
                      rating: review.rating,
                      description: review.description ?? '',
                      )).toList(),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
